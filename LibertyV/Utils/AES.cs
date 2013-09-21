/*
 
    LibertyV - Viewer/Editor for RAGE Package File version 7
    Copyright (C) 2013  koolk <koolkdev at gmail.com>
   
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
  
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
   
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

namespace LibertyV.Utils
{
    public static class AES
    {
        private static RijndaelManaged aes = null;
        private static ICryptoTransform decryptor;
        private static ICryptoTransform decryptor16;
        private static ICryptoTransform encryptor;
        private static ICryptoTransform encryptor16;

        public static byte[] Key = null;

        // Here is an ugly thing. Rockstar using uncommon padding mode - which is to not encrypt the last block 
        // So I need to change the TransformBlock, to not encrypt the last block, and I do it with a proxy
        // Is there a better way?
        private class CryptoTransformProxy : RealProxy
        {
            private readonly ICryptoTransform _instance;
            private readonly bool SixteenRounds;

            private CryptoTransformProxy(ICryptoTransform instance, bool sixteenRounds = false)
                : base(typeof(ICryptoTransform))
            {
                _instance = instance;
                this.SixteenRounds = sixteenRounds;
            }

            public static ICryptoTransform Create(ICryptoTransform instance, bool sixteenRounds = false)
            {
                return (ICryptoTransform)new CryptoTransformProxy(instance, sixteenRounds).GetTransparentProxy();
            }

            private static int TransformBlock16Rounds(ICryptoTransform instance, byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                int res = 0;
                for (int i = 0; i < 16; ++i)
                {
                    res = instance.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
                }
                return inputCount;
            }


            private static byte[] TransformFinalBlock(ICryptoTransform instance, byte[] inputBuffer, int inputOffset, int inputCount)
            {
                if (inputCount >= instance.InputBlockSize)
                {
                    throw new Exception("Unexpected value");
                }
                return inputBuffer;
            }

            public override IMessage Invoke(IMessage msg)
            {
                var methodCall = (IMethodCallMessage)msg;
                var method = (MethodInfo)methodCall.MethodBase;

                try
                {
                    if (SixteenRounds && method.Name == "TransformBlock")
                    {
                        MethodInfo newMethod = typeof(CryptoTransformProxy).GetMethod("TransformBlock16Rounds", BindingFlags.Static | BindingFlags.NonPublic);
                        object[] newParams = new object[methodCall.InArgs.Length + 1];
                        methodCall.InArgs.CopyTo(newParams, 1);
                        newParams[0] = _instance;
                        var result = newMethod.Invoke(null, newParams);
                        return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                    }
                    else if (method.Name == "TransformFinalBlock")
                    {
                        MethodInfo newMethod = typeof(CryptoTransformProxy).GetMethod("TransformFinalBlock", BindingFlags.Static | BindingFlags.NonPublic);
                        object[] newParams = new object[methodCall.InArgs.Length + 1];
                        methodCall.InArgs.CopyTo(newParams, 1);
                        newParams[0] = _instance;
                        var result = newMethod.Invoke(null, newParams);
                        return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                    }
                    else
                    {
                        var result = method.Invoke(_instance, methodCall.InArgs);
                        return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
                    }
                }
                catch (Exception e)
                {
                    if (e is TargetInvocationException && e.InnerException != null)
                    {
                        return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
                    }

                    return new ReturnMessage(e, msg as IMethodCallMessage);
                }
            }
        }

        private static void InitalizeAES()
        {
            aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.Key = AES.Key;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.Zeros;
            decryptor = CryptoTransformProxy.Create(aes.CreateDecryptor());
            decryptor16 = CryptoTransformProxy.Create(aes.CreateDecryptor(), true);
            encryptor = CryptoTransformProxy.Create(aes.CreateEncryptor());
            encryptor16 = CryptoTransformProxy.Create(aes.CreateEncryptor(), true);
        }

        public static byte[] Decrypt(byte[] data)
        {
            if (aes == null)
            {
                InitalizeAES();
            }
            decryptor.TransformBlock(data, 0, (data.Length / 0x10) * 0x10, data, 0);
            return data;
        }

        public static CryptoStream DecryptStream(Stream input, bool sixteenRounds = false)
        {
            if (aes == null)
            {
                InitalizeAES();
            }
            if (sixteenRounds)
            {
                return new CryptoStream(input, decryptor16, CryptoStreamMode.Read);
            }
            else
            {
                return new CryptoStream(input, decryptor, CryptoStreamMode.Read);
            }
        }

        public static CryptoStream EncryptStream(Stream input, bool sixteenRounds = false)
        {
            if (aes == null)
            {
                InitalizeAES();
            }
            if (sixteenRounds)
            {
                return new CryptoStream(input, encryptor16, CryptoStreamMode.Write);
            }
            else
            {
                return new CryptoStream(input, encryptor, CryptoStreamMode.Write);
            }
        }

    }
}
