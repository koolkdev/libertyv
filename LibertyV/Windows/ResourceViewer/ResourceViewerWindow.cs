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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibertyV.Rage.Resources.Types;

namespace LibertyV.Windows.ResourceViewer
{
    public partial class ResourceTypeViewer : Form
    {
        public ResourceTypeViewer()
        {
            InitializeComponent();
        }

        public ResourceTypeViewer(ResourceObject obj, string name = "this")
            : this()
        {
            AddObject(resourcesView.Nodes, name, obj);
        }

        private void AddObject(TreeListNodeCollection ParentNodes, string name, ResourceObject obj)
        {
            TreeListNode node = new TreeListNode();
            node.Text = name;
            node.SubItems.Add(obj.ToString()); // Value
            node.SubItems.Add(obj.Type.Name); // Type

            ParentNodes.Add(node);

            foreach (var member in obj.GetChilds())
            {
                AddObject(node.Nodes, member.Item1, member.Item2);
            }
        }
    }
}
