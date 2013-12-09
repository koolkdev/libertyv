// TreeListView class
// Author: Jon Rista
// Email: jrista@hotmail.com
// 
// TreeListView extends ContainerListView to provide
// a fully-featured hybrid listview that allows the
// first column to behave as a tree.
////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace System.Windows.Forms
{
	#region Enumerations
	#endregion

	#region Event Stuff
	#endregion

	#region TreeListNode
	[DesignTimeVisible(false), TypeConverter("System.Windows.Forms.TreeListNodeConverter")]
	public class TreeListNode: IParentChildList
	{
		#region Event Handlers
		public event MouseEventHandler MouseDown;

		private void OnSubItemsChanged(object sender, ItemsChangedEventArgs e)
		{
			subitems[e.IndexChanged].MouseDown += new MouseEventHandler(OnSubItemMouseDown);
		}

		private void OnSubItemMouseDown(object sender, MouseEventArgs e)
		{
			if (MouseDown != null)
				MouseDown(this, e);
		}

		private void OnSubNodeMouseDown(object sender, MouseEventArgs e)
		{
			if (MouseDown != null)
				MouseDown(sender, e);
		}
		#endregion

		#region Variables
		private Color backcolor = SystemColors.Window;
		private bool ischecked = false;
		private bool focused = false;
		private Font font;
		private Color forecolor = SystemColors.WindowText;
		private int imageindex = 0;
		private int stateimageindex = 0;
		private int index = 0;
		private bool selected = false;
		private ContainerSubListViewItemCollection subitems;
		private object tag;
		private string text;
		private bool styleall = false;
		private bool hovered = false;

		private TreeListNode curChild = null;
		private TreeListNodeCollection nodes;
		private string fullPath = "";
		private bool expanded = false;
		private bool visible = true;

		private TreeListNode parent;
		#endregion

		#region Constructor
		public TreeListNode()
		{
			subitems = new ContainerSubListViewItemCollection();
			subitems.ItemsChanged += new ItemsChangedEventHandler(OnSubItemsChanged);

			nodes = new TreeListNodeCollection();
			nodes.Owner = this;
			nodes.MouseDown += new MouseEventHandler(OnSubNodeMouseDown);
		}
		#endregion

		#region Properties
		[
		Category("Behavior"),
		Description("Sets the parent node of this node.")
		]
		public TreeListNode Parent
		{
			set { parent = value; }
		}

		[
		Category("Data"), 
		Description("The collection of root nodes in the treelist."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(typeof(CollectionEditor), typeof(UITypeEditor))
		]
		public TreeListNodeCollection Nodes
		{
			get { return nodes; }
		}

		[Category("Behavior"), DefaultValue(false)]
		public bool IsExpanded
		{
			get { return expanded; }
			set { expanded = value; }
		}

		[Category("Behavior"), DefaultValue(true)]
		public bool IsVisible
		{
			get { return visible; }
			set { visible = value; }
		}

		[Category("Behavior")]
		public string FullPath
		{
			get { return fullPath; }
		}

		[Category("Appearance")]
		public Color BackColor
		{
			get { return backcolor;	}
			set { backcolor = value; }
		}

		[Category("Behavior")]
		public bool Checked
		{
			get { return ischecked; }
			set { ischecked = value; }
		}

		[Browsable(false)]
		public bool Focused
		{
			get { return focused; }
			set { focused = value; }
		}

		[Category("Appearance")]
		public Font Font
		{
			get { return font; }
			set { font = value; }
		}

		[Category("Appearance")]
		public Color ForeColor
		{
			get { return forecolor; }
			set { forecolor = value; }
		}

		[Category("Behavior")]
		public int ImageIndex
		{
			get { return imageindex; }
			set { imageindex = value; }
		}

		[Browsable(false)]
		public int Index
		{
			get { return index; }
			set { index = value; }
		}

		[Browsable(false)]
		public bool Selected
		{
			get { return selected; }
			set { selected = value; }
		}

		[
		Category("Behavior"),
		Description("The items collection of sub controls."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(typeof(CollectionEditor), typeof(UITypeEditor))		 
		]
		public ContainerSubListViewItemCollection SubItems
		{
			get { return subitems; }
		}

		[Category("Behavior")]
		public int StateImageIndex
		{
			get { return stateimageindex; }
			set { stateimageindex = value; }
		}

		[Browsable(false)]
		public object Tag
		{
			get { return tag; }
			set { tag = value; }
		}

		[Category("Appearance")]
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		[Category("Behavior")]
		public bool UseItemStyleForSubItems
		{
			get { return styleall; }
			set { styleall = value; }
		}

		[Browsable(false)]
		public bool Hovered
		{
			get { return hovered; }
			set { hovered = value; }
		}
		#endregion

		#region Methods
		public void Collapse()
		{
			expanded = false;
		}

		public void CollapseAll()
		{
			for(int i=0; i<nodes.Count; i++)
			{
				nodes[i].CollapseAll();
			}
			Collapse();
		}
		public void Expand()
		{
			expanded = true;
		}

		public void ExpandAll()
		{
			for (int i=0; i<nodes.Count; i++) 
				((TreeListNode)nodes[i]).ExpandAll();

			expanded = true;
		}

		public int GetNodeCount(bool includeSubTrees)
		{
			int c=0;

			if (includeSubTrees)
			{
				for (int n=0; n<nodes.Count; n++)
				{
					c += nodes[n].GetNodeCount(true);
				}
			}
			c += nodes.Count;
			return c;
		}

		public int GetVisibleNodeCount(bool includeSubTrees)
		{
			int c=0;

			if (expanded)
			{
				if (includeSubTrees)
				{
					for (int n=0; n<nodes.Count; n++)
					{
						if (nodes[n].IsExpanded)
							c += nodes[n].GetVisibleNodeCount(true);
					}
				}

				for (int n=0; n<nodes.Count; n++)
				{
					if (nodes[n].IsVisible)
						c++;
				}
			}		

			return c;
		}

		public void Remove()
		{
			int c = nodes.IndexOf(curChild);
			nodes.Remove(curChild);
			if (nodes.Count > 0 && nodes.Count > c)
				curChild = nodes[c];
			else
				curChild = nodes[nodes.Count];
		}

		public void Toggle()
		{
			if (expanded)
				expanded = false;
			else
				expanded = true;
		}
		#endregion

		#region IParentChildList
		public object ParentNode()
		{
			return parent;
		}

		public object PreviousSibling()
		{
			if (parent != null)
			{
				int thisIndex = parent.Nodes[this];
				if (thisIndex > 0)
					return parent.Nodes[thisIndex-1];
			}

			return null;
		}

		public object NextSibling()
		{
			if (parent != null)
			{
				int thisIndex = parent.Nodes[this];
				if (thisIndex < parent.Nodes.Count-1)
					return parent.Nodes[thisIndex+1];
			}

			return null;
		}

		public object FirstChild()
		{
			curChild = Nodes[0];
			return curChild;
		}

		public object LastChild()
		{
			curChild = Nodes[Nodes.Count-1];
			return curChild;
		}

		public object NextChild()
		{
			curChild = (TreeListNode)curChild.NextSibling();
			return curChild;
		}

		public object PreviousChild()
		{
			curChild = (TreeListNode)curChild.PreviousSibling();
			return curChild;
		}
		#endregion
	}

	public class TreeListNodeCollection: CollectionBase
	{
		#region Events
		public event MouseEventHandler MouseDown;
		public event EventHandler NodesChanged;

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (MouseDown != null)
				MouseDown(sender, e);
		}

		private void OnNodesChanged()
		{
			OnNodesChanged(this, new EventArgs());
		}

		private void OnNodesChanged(object sender, EventArgs e)
		{
			if (NodesChanged != null)
				NodesChanged(sender, e);
		}
		#endregion

		#region Variables
		private TreeListNode owner;
		#endregion

		public TreeListNodeCollection()
		{
		}

		public TreeListNodeCollection(TreeListNode owner)
		{
			this.owner = owner;
		}

		public TreeListNode Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		public int TotalCount
		{
			get 
			{
				int tcnt = 0;
				tcnt += List.Count;
				foreach (TreeListNode n in List)
				{
					tcnt += n.Nodes.TotalCount;
				}

				return tcnt;
			}
		}

		#region Implementation
		public TreeListNode this[int index]
		{
			get 
			{ 
				if (List.Count > 0)
				{
					return List[index] as TreeListNode;
				}
				else
					return null;
			}
			set 
			{
				List[index] = value;
				TreeListNode tln = ((TreeListNode)List[index]);
				tln.MouseDown += new MouseEventHandler(OnMouseDown);
				tln.Nodes.NodesChanged += new EventHandler(OnNodesChanged);
				tln.Parent = owner;
				OnNodesChanged();
			}
		}

		public int this[TreeListNode item]
		{
			get { return List.IndexOf(item); }
		}

		public int Add(TreeListNode item)
		{
			item.MouseDown += new MouseEventHandler(OnMouseDown);
			item.Nodes.NodesChanged += new EventHandler(OnNodesChanged);
			item.Parent = owner;
			OnNodesChanged();
			return item.Index = List.Add(item);
		}

		public void AddRange(TreeListNode[] items)
		{
			lock(List.SyncRoot)
			{
				for (int i=0; i<items.Length; i++)
				{
					items[i].MouseDown += new MouseEventHandler(OnMouseDown);
					items[i].Nodes.NodesChanged+= new EventHandler(OnNodesChanged);
					items[i].Parent = owner;
					items[i].Index = List.Add(items[i]);
				}
				OnNodesChanged();
			}
		}

		public void Remove(TreeListNode item)
		{
			List.Remove(item);
		}

		public new void Clear()
		{
			for (int i=0; i<List.Count; i++)
			{
				ContainerSubListViewItemCollection col = ((TreeListNode)List[i]).SubItems;
				for (int j=0; j<col.Count; j++)
				{
					if (col[j].ItemControl != null)
					{
						col[j].ItemControl.Parent = null;
						col[j].ItemControl.Visible = false;
						col[j].ItemControl = null;
					}
				}
				((TreeListNode)List[i]).Nodes.Clear();
			}
			List.Clear();
			OnNodesChanged();
		}

		public int IndexOf(TreeListNode item)
		{
			return List.IndexOf(item);
		}
		#endregion
	}
	#endregion

	#region TreeListView
	/// <summary>
	/// TreeListView provides a hybrid listview whos first
	/// column can behave as a treeview. This control extends
	/// ContainerListView, allowing subitems to contain 
	/// controls.
	/// </summary>
	public class TreeListView : System.Windows.Forms.ContainerListView
	{
		#region Events
		protected override void OnSubControlMouseDown(object sender, MouseEventArgs e)
		{
			TreeListNode node = (TreeListNode)sender;
			
			UnselectNodes(nodes);
			
			node.Focused = true;
			node.Selected = true;				
			//focusedIndex = firstSelected = i;

			if (e.Clicks >= 2)
				node.Toggle();

			// set selected items and indices collections							
			//selectedIndices.Add(i);						
			//selectedItems.Add(items[i]);
			Invalidate(ClientRectangle);
		}

		protected virtual void OnNodesChanged(object sender, EventArgs e)
		{
			AdjustScrollbars();
		}
		#endregion

		#region Variables
		protected TreeListNodeCollection nodes;
		protected int indent = 19;
		protected int itemheight = 20;
		protected bool showlines = false, showrootlines = false, showplusminus = true;

		protected ListDictionary pmRects;
		protected ListDictionary nodeRowRects;

		protected bool alwaysShowPM = false;

		protected Bitmap bmpMinus, bmpPlus;

		private TreeListNode curNode;
		private TreeListNode virtualParent;

		private TreeListNodeCollection selectedNodes;

		private bool mouseActivate = false;

		private bool allCollapsed = false;
		#endregion

		#region Constructor
		public TreeListView(): base()
		{
			virtualParent = new TreeListNode();

			nodes = virtualParent.Nodes;
			nodes.Owner = virtualParent;
			nodes.MouseDown += new MouseEventHandler(OnSubControlMouseDown);
			nodes.NodesChanged += new EventHandler(OnNodesChanged);

			selectedNodes = new TreeListNodeCollection();

			nodeRowRects = new ListDictionary();
			pmRects = new ListDictionary();	

			// Use reflection to load the
			// embedded bitmaps for the
			// styles plus and minus icons
            Assembly myAssembly = Assembly.GetAssembly(Type.GetType("System.Windows.Forms.TreeListView"));
            Stream bitmapStream1 = myAssembly.GetManifestResourceStream("LibertyV.Utils.TreeView.Resources.tv_minus.bmp");
			bmpMinus = new Bitmap(bitmapStream1);

            Stream bitmapStream2 = myAssembly.GetManifestResourceStream("LibertyV.Utils.TreeView.Resources.tv_plus.bmp");
			bmpPlus = new Bitmap(bitmapStream2);
		}
		#endregion

		#region Properties
		[
		Browsable(false)
		]
		public TreeListNodeCollection SelectedNodes
		{
			get { return GetSelectedNodes(virtualParent); }
		}

		[
		Category("Behavior"),
		Description("Determins wether an item is activated or expanded by a double click."),
		DefaultValue(false)
		]
		public bool MouseActivte
		{
			get { return mouseActivate; }
			set { mouseActivate = value; }
		}

		[
		Category("Behavior"),
		Description("Specifies wether to always show plus/minus signs next to each node."),
		DefaultValue(false)
		]
		public bool AlwaysShowPlusMinus
		{
			get { return alwaysShowPM; }
			set { alwaysShowPM = value; }
		}

		[
		Category("Data"), 
		Description("The collection of root nodes in the treelist."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(typeof(CollectionEditor), typeof(UITypeEditor))
		]
		public TreeListNodeCollection Nodes
		{
			get { return nodes; }
		}

		[Browsable(false)]
		public override ContainerListViewItemCollection Items
		{
			get { return items; }
		}

		[
		Category("Behavior"),
		Description("The indentation of child nodes in pixels."),
		DefaultValue(19)
		]
		public int Indent
		{
			get { return indent; }
			set { indent = value; }
		}

		[
		Category("Appearance"),
		Description("The height of every item in the treelistview."),
		DefaultValue(18)
		]
		public int ItemHeight
		{
			get { return itemheight; }
			set { itemheight = value; }
		}

		[
		Category("Behavior"),
		Description("Indicates wether lines are shown between sibling nodes and between parent and child nodes."),
		DefaultValue(false)
		]
		public bool ShowLines
		{
			get { return showlines; }
			set { showlines = value; }
		}

		[
		Category("Behavior"),
		Description("Indicates wether lines are shown between root nodes."),
		DefaultValue(false)
		]
		public bool ShowRootLines
		{
			get { return showrootlines; }
			set { showrootlines = value; }
		}

		[
		Category("Behavior"),
		Description("Indicates wether plus/minus signs are shown next to parent nodes."),
		DefaultValue(true)
		]
		public bool ShowPlusMinus
		{
			get { return showplusminus; }
			set { showplusminus = value; }
		}
		#endregion

		#region Overrides
		public override bool PreProcessMessage(ref Message msg)
		{
			if (msg.Msg == WM_KEYDOWN)
			{
				if (nodes.Count > 0)
				{
					if (curNode != null)
					{
						Keys keyData = ((Keys) (int) msg.WParam) | ModifierKeys;
						Keys keyCode = ((Keys) (int) msg.WParam);

						if (keyCode == Keys.Left)	// collapse current node or move up to parent
						{
							if (curNode.IsExpanded)
							{
								curNode.Collapse();
							}
							else if (curNode.ParentNode() != null)
							{
								TreeListNode t = (TreeListNode)curNode.ParentNode();
								if (t.ParentNode() != null) // never select virtualParent node
								{
									if (!multiSelect)
										curNode.Selected = false;
									//else
									curNode.Focused = false;
									curNode = (TreeListNode)curNode.ParentNode();
									if (!multiSelect)
										curNode.Selected = true;
									//else
									curNode.Focused = true;
								}
							}

							Invalidate();
							return true;
						}
						else if (keyCode == Keys.Right) // expand current node or move down to first child
						{
							if (!curNode.IsExpanded)
							{
								curNode.Expand();
							}
							else if (curNode.IsExpanded && curNode.GetNodeCount(false) > 0)
							{
								if (!multiSelect)
									curNode.Selected = false;
								//else
								curNode.Focused = false;
								curNode = (TreeListNode)curNode.FirstChild();
								if (!multiSelect)
									curNode.Selected = true;
								//else
								curNode.Focused = true;
							}

							Invalidate();
							return true;
						}

						else if (keyCode == Keys.Up)
						{
							if (curNode.PreviousSibling() == null && curNode.ParentNode() != null)
							{
								TreeListNode t = (TreeListNode)curNode.ParentNode();
								if (t.ParentNode() != null) // never select virtualParent node
								{
									if (!multiSelect)
										curNode.Selected = false;
									//else
									curNode.Focused = false;
									curNode = (TreeListNode)curNode.ParentNode();
									if (!multiSelect)
										curNode.Selected = true;
									//else
									curNode.Focused = true;
								}

								Invalidate();
								return true;
							}
							else if (curNode.PreviousSibling() != null)
							{
								TreeListNode t = (TreeListNode)curNode.PreviousSibling();
								if (t.GetNodeCount(false) > 0 && t.IsExpanded)
								{
									do
									{
										t = (TreeListNode)t.LastChild();
										if (!t.IsExpanded)
										{
											if (!multiSelect)
												curNode.Selected = false;
											//else
											curNode.Focused = false;
											curNode = t;
											if (!multiSelect)
												curNode.Selected = true;
											//else
											curNode.Focused = true;
										}
									} while (t.GetNodeCount(false) > 0 && t.IsExpanded);
								}
								else
								{
									if (!multiSelect)
										curNode.Selected = false;
									//else
									curNode.Focused = false;
									curNode = (TreeListNode)curNode.PreviousSibling();
									if (!multiSelect)
										curNode.Selected = true;
									//else
									curNode.Focused = true;
								}

								Invalidate();
								return true;
							}
						}
						else if (keyCode == Keys.Down)
						{
							if (curNode.IsExpanded && curNode.GetNodeCount(false) > 0)
							{
								if (!multiSelect)
									curNode.Selected = false;
								//else
								curNode.Focused = false;
								curNode = (TreeListNode)curNode.FirstChild();							
								if (!multiSelect)
									curNode.Selected = true;
								//else
								curNode.Focused = true;
							}
							else if (curNode.NextSibling() == null && curNode.ParentNode() != null)
							{
								TreeListNode t = curNode;
								do
								{
									t = (TreeListNode)t.ParentNode();
									if (t.NextSibling() != null)
									{
										if (!multiSelect)
											curNode.Selected = false;
										//else
										curNode.Focused = false;
										curNode = (TreeListNode)t.NextSibling();
										if (!multiSelect)
											curNode.Selected = true;
										//else
										curNode.Focused = true;

										break;
									}	
								} while (t.NextSibling() == null && t.ParentNode() != null);
							}						
							else if (curNode.NextSibling() != null)
							{
								if (!multiSelect)
									curNode.Selected = false;
								//else
								curNode.Focused = false;
								curNode = (TreeListNode)curNode.NextSibling();							
								if (!multiSelect)
									curNode.Selected = true;
								//else
								curNode.Focused = true;
							}

							Invalidate();
							return true;
						}						
					}
				}
			}

			return base.PreProcessMessage(ref msg);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			AdjustScrollbars();
			
			Invalidate();
		}


		private void AutoSetColWidth(TreeListNodeCollection nodes, ref int mwid, ref int twid, int i)
		{
			for (int j=0; j<nodes.Count; j++)
			{
				if (i > 0)
					twid = GetStringWidth(nodes[j].SubItems[i-1].Text);
				else
					twid = GetStringWidth(nodes[j].Text);
				twid += 5;
				if (twid > mwid)
					mwid = twid;

				if (nodes[j].Nodes.Count > 0)
				{
					AutoSetColWidth(nodes[j].Nodes, ref mwid, ref twid, i);
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			for (int i=0; i<columns.Count; i++)
			{
				if (columnSizeRects.Length > 0 && MouseInRect(e, columnSizeRects[i]))
				{
					// autosize column
					if (e.Clicks == 2 && e.Button == MouseButtons.Left)
					{
						int mwid = 0;
						int twid = 0;

						AutoSetColWidth(nodes, ref mwid, ref twid, i);					
								
						twid = GetStringWidth(columns[i].Text);
						if (twid > mwid)
							mwid = twid;

						mwid += 5;

						if (columns[i].Image != null)
							mwid += 18;

						columns[i].Width = mwid;
						GenerateColumnRects();
					}
						// scale column
					else
					{
						colScaleMode = true;
						colScaleWid = columnRects[i].Width;
						scaledCol = i;
					}
				}
			}

			if (MouseInRect(e, rowsRect))
			{
				TreeListNode cnode;
				// check if a nodes plus/minus has been clicked
				cnode = NodePlusClicked(e);
				if (cnode != null)
				{
					cnode.Toggle();
					AdjustScrollbars();
					Invalidate(ClientRectangle);
					return;
				}

				if (e.Button == MouseButtons.Left)
				{
					// check if a noderow has been clicked
					if (multiSelectMode == MultiSelectMode.Single)
					{
						UnselectNodes(nodes);
						//selectedNodes.Clear();
					
						cnode = NodeInNodeRow(e);
						if (cnode != null)
						{
							cnode.Focused = true;
							cnode.Selected = true;
							curNode = cnode;

							//selectedNodes.Add(curNode);

							if (e.Clicks == 2 && !mouseActivate)
							{
								cnode.Toggle();
								AdjustScrollbars();
							}
							else if (e.Clicks == 2 && mouseActivate)
								OnItemActivate(new EventArgs());					
						}
						else if (curNode != null)
						{
							curNode.Focused = false;
							curNode.Selected = false;
						}
						Invalidate();
					}
					else if (multiSelectMode == MultiSelectMode.Range)
					{
						// to be implemented at a later date
					}
					else if (multiSelectMode == MultiSelectMode.Selective)
					{
						UnfocusNodes(nodes);

						cnode = NodeInNodeRow(e);
						if (cnode != null)
						{
							if (cnode.Selected)
							{
								// remove node from collection of selected nodes
								//selectedNodes.Remove(curNode);

								cnode.Focused = false;
								cnode.Selected = false;
								curNode = null;
							}
							else
							{
								cnode.Focused = true;
								cnode.Selected = true;
								curNode = cnode;

								// add node to collection of selected nodes
								//selectedNodes.Add(curNode);
							}

							if (e.Clicks == 2 && !mouseActivate)
							{
								cnode.Toggle();
								AdjustScrollbars();
							}
							else if (e.Clicks == 2 && mouseActivate)
								OnItemActivate(new EventArgs());

							Invalidate();
						}

					}
				}
			}			
		}


		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.KeyCode == Keys.F5)
			{
				if (allCollapsed)
					ExpandAll();
				else
					CollapseAll();
			}
		}


		int rendcnt = 0;
		protected override void DrawRows(Graphics g, Rectangle r)
		{
			// render listview item rows
			int i;
			int totalRend = 0, childCount = 0;

			int maxrend = ClientRectangle.Height/itemheight+1;
			int prior = vscrollBar.Value/itemheight-1;
			if (prior < 0)
				prior = 0;
			int nodesprior = 0;
			int rootstart = 0;

			if (prior > 0)
			{
				int nodescur;
				for (i=0; i<nodes.Count; i++)
				{
					nodescur = 0;
					if (nodes[i].IsExpanded)
						nodescur = nodes[i].GetVisibleNodeCount(true);
					nodesprior += nodescur;
					nodesprior++;
					if (nodesprior > prior)
					{
						nodesprior -= nodescur+1;
						rootstart = i;
						break;
					}
				}
			}

			totalRend = nodesprior;
			rendcnt = 0;

			nodeRowRects.Clear();
			pmRects.Clear();

			for (i=rootstart; i<nodes.Count && rendcnt<maxrend; i++)
			{
				RenderNodeRows(nodes[i], g, r, 0, i, ref totalRend, ref childCount, nodes.Count);
			}
		}
		#endregion

		#region Helper Functions
		private int vsize, hsize;
		public override void AdjustScrollbars()
		{
			if (nodes.Count > 0 || columns.Count > 0 && !colScaleMode)
			{
				allColsWidth = 0;
				for (int i=0; i<columns.Count; i++)
				{
					allColsWidth += columns[i].Width;
				}

				allRowsHeight = 0;
				for (int i=0; i<nodes.Count; i++)
				{
					allRowsHeight += itemheight + itemheight*((TreeListNode)nodes[i]).GetVisibleNodeCount(true);
				}

				vsize = vscrollBar.Width;
				hsize = hscrollBar.Height;

				hscrollBar.Left = this.ClientRectangle.Left+2;
				hscrollBar.Width = this.ClientRectangle.Width-vsize-4;
				hscrollBar.Top = this.ClientRectangle.Top+this.ClientRectangle.Height-hscrollBar.Height-2;
				hscrollBar.Maximum = allColsWidth;			
				if (allColsWidth > this.ClientRectangle.Width-4-vsize)
				{
					hscrollBar.Show();
					hsize = hscrollBar.Height;
				}
				else
				{
					hscrollBar.Hide();
					hscrollBar.Value = 0;
					hsize = 0;
				}

				vscrollBar.Left = this.ClientRectangle.Left+this.ClientRectangle.Width-vscrollBar.Width-2;
				vscrollBar.Top = this.ClientRectangle.Top+headerBuffer+2;
				vscrollBar.Height = this.ClientRectangle.Height-hsize-headerBuffer-4;
				vscrollBar.Maximum = allRowsHeight;
				vscrollBar.LargeChange = (this.ClientRectangle.Height-headerBuffer-hsize-4 > 0 ? this.ClientRectangle.Height-headerBuffer-hsize-4 : 0);
				if (allRowsHeight > this.ClientRectangle.Height-headerBuffer-4-hsize)
				{
					vscrollBar.Show();
					vsize = vscrollBar.Width;
				}
				else
				{
					vscrollBar.Hide();
					vscrollBar.Value = 0;
					vsize = 0;
				}

				hscrollBar.Width = this.ClientRectangle.Width-vsize-4;
				hscrollBar.LargeChange = (this.ClientRectangle.Width - vsize - 4 > 0 ? this.ClientRectangle.Width - vsize - 4 : 0);
				hscrollBar.Top = this.ClientRectangle.Top+this.ClientRectangle.Height-hscrollBar.Height-2;	
				if (allColsWidth > this.ClientRectangle.Width-4-vsize)
				{
					hscrollBar.Show();
				}
				else
				{
					hscrollBar.Hide();
					hscrollBar.Value = 0;
					hsize = 0;
				}
			}
		}

		private void UnfocusNodes(TreeListNodeCollection nodecol)
		{
			for (int i=0; i<nodecol.Count; i++)
			{
				UnfocusNodes(nodecol[i].Nodes);
				nodecol[i].Focused = false;
			}
		}

		private void UnselectNodes(TreeListNodeCollection nodecol)
		{
			for (int i=0; i<nodecol.Count; i++)
			{
				UnselectNodes(nodecol[i].Nodes);
				nodecol[i].Focused = false;
				nodecol[i].Selected = false;
			}
		}

		private TreeListNode NodeInNodeRow(MouseEventArgs e)
		{
			IEnumerator ek = nodeRowRects.Keys.GetEnumerator();
			IEnumerator ev = nodeRowRects.Values.GetEnumerator();

			while (ek.MoveNext() && ev.MoveNext())
			{
				Rectangle r = (Rectangle)ek.Current;

				if (r.Left <= e.X && r.Left+r.Width >= e.X
					&& r.Top <= e.Y && r.Top+r.Height >= e.Y)
				{
					return (TreeListNode)ev.Current;
				}
			}

			return null;
		}

		private TreeListNode NodePlusClicked(MouseEventArgs e)
		{
			IEnumerator ek = pmRects.Keys.GetEnumerator();
			IEnumerator ev = pmRects.Values.GetEnumerator();

			while (ek.MoveNext() && ev.MoveNext())
			{
				Rectangle r = (Rectangle)ek.Current;

				if (r.Left <= e.X && r.Left+r.Width >= e.X
					&& r.Top <= e.Y && r.Top+r.Height >= e.Y)
				{
					return (TreeListNode)ev.Current;
				}
			}

			return null;
		}

		private void RenderNodeRows(TreeListNode node, Graphics g, Rectangle r, int level, int index, ref int totalRend, ref int childCount, int count)
		{
			if (node.IsVisible)
			{
				int eb = 10;	// edge buffer				

				// only render if row is visible in viewport
				if (((r.Top+itemheight*totalRend+eb/4-vscrollBar.Value+itemheight > r.Top) 
					&& (r.Top+itemheight*totalRend+eb/4-vscrollBar.Value < r.Top+r.Height)))
				{
					rendcnt++;
					int lb = 0;		// level buffer
					int ib = 0;		// icon buffer
					int hb = headerBuffer;	// header buffer	
					Pen linePen = new Pen(SystemBrushes.ControlDark, 1.0f);
					Pen PMPen = new Pen(SystemBrushes.ControlDark, 1.0f);
					Pen PMPen2 = new Pen(new SolidBrush(Color.Black), 1.0f);

					linePen.DashStyle = DashStyle.Dot;

					// add space for plis/minus icons and/or root lines to the edge buffer
					if (showrootlines || showplusminus)
					{
						eb += 10;
					}

					// set level buffer
					lb = indent*level;

					// set icon buffer
					if ((node.Selected || node.Focused) && stateImageList != null)
					{
						if (node.ImageIndex >= 0 && node.ImageIndex < stateImageList.Images.Count)
						{
							stateImageList.Draw(g, r.Left+lb+eb+2-hscrollBar.Value, r.Top+hb+itemheight*totalRend+eb/4-2-vscrollBar.Value, 16, 16, node.ImageIndex);
							ib = 18;
						}
					}
					else
					{
						if (smallImageList != null && node.ImageIndex >= 0 && node.ImageIndex < smallImageList.Images.Count)
						{
							smallImageList.Draw(g, r.Left+lb+eb+2-hscrollBar.Value, r.Top+hb+itemheight*totalRend+eb/4-2-vscrollBar.Value, 16, 16, node.ImageIndex);
							ib = 18;
						}
					}

					// add a rectangle to the node row rectangles
					Rectangle sr = new Rectangle(r.Left+lb+ib+eb+4-hscrollBar.Value, r.Top+hb+itemheight*totalRend+2-vscrollBar.Value, allColsWidth-(lb+ib+eb+4), itemheight);
					nodeRowRects.Add(sr, node);

					// render per-item background
					if (node.BackColor != this.BackColor)
					{
						if (node.UseItemStyleForSubItems)
							g.FillRectangle(new SolidBrush(node.BackColor), r.Left+lb+ib+eb+4-hscrollBar.Value, r.Top+hb+itemheight*totalRend+2-vscrollBar.Value, allColsWidth-(lb+ib+eb+4), itemheight);
						else
							g.FillRectangle(new SolidBrush(node.BackColor), r.Left+lb+ib+eb+4-hscrollBar.Value, r.Top+hb+itemheight*totalRend+2-vscrollBar.Value, columns[0].Width-(lb+ib+eb+4), itemheight);
					}

					g.Clip = new Region(sr);

					// render selection and focus
					if (node.Selected && isFocused)
					{
						g.FillRectangle(new SolidBrush(rowSelectColor), sr);
					}
					else if (node.Selected && !isFocused && !hideSelection)
					{
						g.FillRectangle(SystemBrushes.Control, sr);
					}
					else if (node.Selected && !isFocused && hideSelection)
					{
						ControlPaint.DrawFocusRectangle(g, sr);
					}

					if (node.Focused && ((isFocused && multiSelect) || !node.Selected))
					{
						ControlPaint.DrawFocusRectangle(g, sr);
					}
			
					g.Clip = new Region(new Rectangle(r.Left+2-hscrollBar.Value, r.Top+hb+2, columns[0].Width, r.Height-hb-4));

					// render root lines if visible
					if (r.Left+eb-hscrollBar.Value > r.Left)
					{						
						if (showrootlines && level == 0)
						{
							if (index == 0)
							{
								g.DrawLine(linePen, r.Left+eb/2-hscrollBar.Value, r.Top+eb/2+hb-vscrollBar.Value, r.Left+eb-hscrollBar.Value, r.Top+eb/2+hb-vscrollBar.Value);
								g.DrawLine(linePen, r.Left+eb/2-hscrollBar.Value, r.Top+eb/2+hb-vscrollBar.Value, r.Left+eb/2-hscrollBar.Value, r.Top+eb+hb-vscrollBar.Value);
							}
							else if (index == count-1)
							{
								g.DrawLine(linePen, r.Left+eb/2-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value, r.Left+eb-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value);
								g.DrawLine(linePen, r.Left+eb/2-hscrollBar.Value, r.Top+hb+itemheight*(totalRend)-vscrollBar.Value, r.Left+eb/2-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value);
							}
							else
							{
								g.DrawLine(linePen, r.Left+eb/2-hscrollBar.Value, r.Top+eb+hb+itemheight*(totalRend)-eb/2-vscrollBar.Value, r.Left+eb-hscrollBar.Value, r.Top+eb+hb+itemheight*(totalRend)-eb/2-vscrollBar.Value);
								g.DrawLine(linePen, r.Left+eb/2-hscrollBar.Value, r.Top+eb+hb+itemheight*(totalRend-1)-vscrollBar.Value, r.Left+eb/2-hscrollBar.Value, r.Top+eb+hb+itemheight*(totalRend)-vscrollBar.Value);
							}

							if (childCount > 0)
								g.DrawLine(linePen, r.Left+eb/2-hscrollBar.Value, r.Top+hb+itemheight*(totalRend-childCount)-vscrollBar.Value, r.Left+eb/2-hscrollBar.Value, r.Top+hb+itemheight*(totalRend)-vscrollBar.Value);
						}
					}

					// render child lines if visible
					if (r.Left+lb+eb-hscrollBar.Value > r.Left)
					{						
						if (showlines && level > 0)
						{
							if (index == count-1)
							{
								g.DrawLine(linePen, r.Left+lb+eb/2-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value, r.Left+lb+eb-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value);
								g.DrawLine(linePen, r.Left+lb+eb/2-hscrollBar.Value, r.Top+hb+itemheight*(totalRend)-vscrollBar.Value, r.Left+lb+eb/2-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value);
							}
							else
							{
								g.DrawLine(linePen, r.Left+lb+eb/2-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value, r.Left+lb+eb-hscrollBar.Value, r.Top+eb/2+hb+itemheight*(totalRend)-vscrollBar.Value);
								g.DrawLine(linePen, r.Left+lb+eb/2-hscrollBar.Value, r.Top+hb+itemheight*(totalRend)-vscrollBar.Value, r.Left+lb+eb/2-hscrollBar.Value, r.Top+eb+hb+itemheight*(totalRend)-vscrollBar.Value);
							}

							if (childCount > 0)
								g.DrawLine(linePen, r.Left+lb+eb/2-hscrollBar.Value, r.Top+hb+itemheight*(totalRend-childCount)-vscrollBar.Value, r.Left+lb+eb/2-hscrollBar.Value, r.Top+hb+itemheight*(totalRend)-vscrollBar.Value);
						}
					}

					// render +/- signs if visible
					if (r.Left+lb+eb/2+5-hscrollBar.Value > r.Left)
					{
						if (showplusminus && (node.GetNodeCount(false) > 0 || alwaysShowPM))
						{
							if (index == 0 && level == 0)
							{
								RenderPlus(g, r.Left+lb+eb/2-4-hscrollBar.Value, r.Top+hb+eb/2-4-vscrollBar.Value, 8, 8, node);
							}
							else if (index == count-1)
							{

								RenderPlus(g, r.Left+lb+eb/2-4-hscrollBar.Value, r.Top+hb+itemheight*totalRend+eb/2-4-vscrollBar.Value, 8, 8, node);
							}
							else
							{
								RenderPlus(g, r.Left+lb+eb/2-4-hscrollBar.Value, r.Top+hb+itemheight*totalRend+eb/2-4-vscrollBar.Value, 8, 8, node);
							}
						}
					}

					// render text if visible
					if (r.Left+columns[0].Width-hscrollBar.Value > r.Left)
					{
						if (node.Selected && isFocused)
							g.DrawString(TruncatedString(node.Text, columns[0].Width, lb+eb+ib+6, g), Font, SystemBrushes.HighlightText, (float)(r.Left+lb+ib+eb+4-hscrollBar.Value), (float)(r.Top+hb+itemheight*totalRend+eb/4-vscrollBar.Value));
						else
							g.DrawString(TruncatedString(node.Text, columns[0].Width, lb+eb+ib+6, g), Font, new SolidBrush(node.ForeColor), (float)(r.Left+lb+ib+eb+4-hscrollBar.Value), (float)(r.Top+hb+itemheight*totalRend+eb/4-vscrollBar.Value));
					}

					// render subitems
					int j;
					int last = 0;
					if (columns.Count > 0)
					{
						for (j=0; j<node.SubItems.Count && j<columns.Count; j++)
						{
							last += columns[j].Width;
							g.Clip = new Region(new Rectangle(last+6-hscrollBar.Value, r.Top+headerBuffer+2, (last+columns[j+1].Width > r.Width-6 ? r.Width-6 : columns[j+1].Width-6), r.Height-5));
							if (node.SubItems[j].ItemControl != null)
							{
								Control c = node.SubItems[j].ItemControl;
								c.Location = new Point(r.Left+last+4-hscrollBar.Value, r.Top+(itemheight*totalRend)+headerBuffer+4-vscrollBar.Value);
								c.ClientSize = new Size(columns[j+1].Width-6, rowHeight-4);
								c.Parent = this;
							}						
							else
							{
								string sp = "";
								if (columns[j+1].TextAlign == HorizontalAlignment.Left)
								{
									if (node.Selected && isFocused)
										g.DrawString(TruncatedString(node.SubItems[j].Text, columns[j+1].Width, 9, g), this.Font, SystemBrushes.HighlightText, (float)(last+6-hscrollBar.Value), (float)(r.Top+(itemheight*totalRend)+headerBuffer+4-vscrollBar.Value));
									else
										g.DrawString(TruncatedString(node.SubItems[j].Text, columns[j+1].Width, 9, g), this.Font, (node.UseItemStyleForSubItems ? new SolidBrush(node.ForeColor) : SystemBrushes.WindowText), (float)(last+6-hscrollBar.Value), (float)(r.Top+(itemheight*totalRend)+headerBuffer+4-vscrollBar.Value));
								}
								else if (columns[j+1].TextAlign == HorizontalAlignment.Right)
								{
									sp = TruncatedString(node.SubItems[j].Text, columns[j+1].Width, 9, g);
									if (node.Selected && isFocused)
										g.DrawString(sp, this.Font, SystemBrushes.HighlightText, (float)(last+columns[j+1].Width-Helpers.StringTools.MeasureDisplayStringWidth(g, sp, this.Font)-4-hscrollBar.Value), (float)(r.Top+(itemheight*totalRend)+headerBuffer+4-vscrollBar.Value));
									else
										g.DrawString(sp, this.Font, (node.UseItemStyleForSubItems ? new SolidBrush(node.ForeColor) : SystemBrushes.WindowText), (float)(last+columns[j+1].Width-Helpers.StringTools.MeasureDisplayStringWidth(g, sp, this.Font)-4-hscrollBar.Value), (float)(r.Top+(itemheight*totalRend)+headerBuffer+4-vscrollBar.Value));
								}
								else
								{
									sp = TruncatedString(node.SubItems[j].Text, columns[j+1].Width, 9, g);
									if (node.Selected && isFocused)
										g.DrawString(sp, this.Font, SystemBrushes.HighlightText, (float)(last+(columns[j+1].Width/2)-(Helpers.StringTools.MeasureDisplayStringWidth(g, sp, this.Font)/2)-hscrollBar.Value), (float)(r.Top+(itemheight*totalRend)+headerBuffer+4-vscrollBar.Value));
									else
										g.DrawString(sp, this.Font, (node.UseItemStyleForSubItems ? new SolidBrush(node.ForeColor) : SystemBrushes.WindowText), (float)(last+(columns[j+1].Width/2)-(Helpers.StringTools.MeasureDisplayStringWidth(g, sp, this.Font)/2)-hscrollBar.Value), (float)(r.Top+(itemheight*totalRend)+headerBuffer+4-vscrollBar.Value));
								}
							}
						}
					}
				}

				// increment number of rendered nodes
				totalRend++;

				// render child nodes
				if (node.IsExpanded)
				{
					childCount = 0;
					for (int n=0; n<node.GetNodeCount(false); n++)
					{
						RenderNodeRows(node.Nodes[n], g, r, level+1, n, ref totalRend, ref childCount, node.Nodes.Count);
					}
				}

				childCount = node.GetVisibleNodeCount(true);
				
			}
			else
			{
				childCount = 0;
			}
		}

		private void RenderPlus(Graphics g, int x, int y, int w, int h, TreeListNode node)
		{
			if (VisualStyles)
			{
				if (node.IsExpanded)
					g.DrawImage(bmpMinus, x, y);
				else
					g.DrawImage(bmpPlus, x, y);
			}
			else
			{
				g.DrawRectangle(new Pen(SystemBrushes.ControlDark),x, y, w, h);
				g.FillRectangle(new SolidBrush(Color.White), x+1, y+1, w-1, h-1);
				g.DrawLine(new Pen(new SolidBrush(Color.Black)), x+2, y+4, x+w-2, y+4);			

				if (!node.IsExpanded)
					g.DrawLine(new Pen(new SolidBrush(Color.Black)), x+4, y+2, x+4, y+h-2);
			}

			pmRects.Add(new Rectangle(x, y, w, h), node);
		}
		#endregion

		#region Methods
		public void CollapseAll()
		{
			foreach (TreeListNode node in nodes)
			{
				node.CollapseAll();
			}
			allCollapsed = true;
			AdjustScrollbars();
			Invalidate();
		}

		public void ExpandAll()
		{
			foreach (TreeListNode node in nodes)
			{
				node.ExpandAll();
			}
			allCollapsed = false;
			AdjustScrollbars();
			Invalidate();
		}

		public TreeListNode GetNodeAt(int x, int y)
		{
			// To be added
			return null;
		}

		public TreeListNode GetNodeAt(Point pt)
		{
			// To be added
			return null;
		}
		
		private TreeListNodeCollection GetSelectedNodes(TreeListNode node)
		{
			TreeListNodeCollection list = new TreeListNodeCollection();

			for (int i=0; i<node.Nodes.Count; i++)
			{
				// check if current node is selected
				if (node.Nodes[i].Selected)
				{
					list.Add(node.Nodes[i]);
				}

				// chech if node is expanded and has
				// selected children
				if (node.Nodes[i].IsExpanded)
				{
					TreeListNodeCollection list2 = GetSelectedNodes(node.Nodes[i]);
					for (int j=0; j<list2.Count; j++)
					{
						list.Add(list2[i]);
					}
				}
			}

			return list;
		}

		public int GetNodeCount()
		{
			int c = 0;

			c += nodes.Count;
			foreach (TreeListNode node in nodes)
			{
				c += GetNodeCount(node);
			}

			return c;
		}

		public int GetNodeCount(TreeListNode node)
		{
			int c = 0;
			c += node.Nodes.Count;
			foreach (TreeListNode n in node.Nodes)
			{
				c += GetNodeCount(n);
			}

			return c;
		}
		#endregion
	}
	#endregion

	#region Type Converters
	public class TreeListNodeConverter: TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is TreeListNode)
			{
				TreeListNode tln = (TreeListNode)value;

				ConstructorInfo ci = typeof(TreeListNode).GetConstructor(new Type[] {});
				if (ci != null)
				{
					return new InstanceDescriptor(ci, null, false);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	#endregion
}
