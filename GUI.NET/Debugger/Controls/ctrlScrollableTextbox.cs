﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mesen.GUI.Debugger
{
	public partial class ctrlScrollableTextbox : UserControl
	{
		public new event MouseEventHandler MouseUp
		{
			add { this.ctrlTextbox.MouseUp += value; }
			remove { this.ctrlTextbox.MouseUp -= value; }
		}

		public new event MouseEventHandler MouseMove
		{
			add { this.ctrlTextbox.MouseMove += value; }
			remove { this.ctrlTextbox.MouseMove -= value; }
		}

		public new event MouseEventHandler MouseDown
		{
			add { this.ctrlTextbox.MouseDown += value; }
			remove { this.ctrlTextbox.MouseDown -= value; }
		}

		public event EventHandler FontSizeChanged;

		public ctrlScrollableTextbox()
		{
			InitializeComponent();

			this.ctrlTextbox.ShowLineNumbers = true;
			this.ctrlTextbox.ShowLineInHex = true;
			this.vScrollBar.ValueChanged += vScrollBar_ValueChanged;
			this.ctrlTextbox.ScrollPositionChanged += ctrlTextbox_ScrollPositionChanged;

			new ToolTip().SetToolTip(picCloseSearch, "Close");
			new ToolTip().SetToolTip(picSearchNext, "Find Next (F3)");
			new ToolTip().SetToolTip(picSearchPrevious, "Find Previous (Shift-F3)");
		}

		public float FontSize
		{
			get { return this.ctrlTextbox.Font.SizeInPoints; }
			set
			{
				if(value >= 6 && value <= 20) {
					this.ctrlTextbox.Font = new Font("Consolas", value);
					this.ctrlTextbox.Invalidate();

					if(this.FontSizeChanged != null) {
						this.FontSizeChanged(this, null);
					}
				}
			}
		}

		public string GetWordUnderLocation(Point position, bool useCompareText = false)
		{
			return this.ctrlTextbox.GetWordUnderLocation(position, useCompareText);
		}

		private void ctrlTextbox_ScrollPositionChanged(object sender, EventArgs e)
		{
			this.vScrollBar.Value = this.ctrlTextbox.ScrollPosition;
		}

		public void ClearLineStyles()
		{
			this.ctrlTextbox.ClearLineStyles();
		}

		public void SetLineColor(int lineNumber, Color? fgColor = null, Color? bgColor = null, Color? outlineColor = null, LineSymbol symbol = LineSymbol.None)
		{
			this.ctrlTextbox.SetLineColor(lineNumber, fgColor, bgColor, outlineColor, symbol);
		}

		public int GetLineIndex(int lineNumber)
		{
			return this.ctrlTextbox.GetLineIndex(lineNumber);
		}

		public int GetLineIndexAtPosition(int yPos)
		{
			return this.ctrlTextbox.GetLineIndexAtPosition(yPos);
		}

		public int GetLineNumber(int lineIndex)
		{
			return this.ctrlTextbox.GetLineNumber(lineIndex);
		}

		public int GetLineNumberAtPosition(int yPos)
		{
			return this.GetLineNumber(this.GetLineIndexAtPosition(yPos));
		}

		public void ScrollToLineIndex(int lineIndex)
		{
			this.ctrlTextbox.ScrollToLineIndex(lineIndex);
		}

		public void ScrollToLineNumber(int lineNumber)
		{
			this.ctrlTextbox.ScrollToLineNumber(lineNumber);
		}

		public int CurrentLine
		{
			get { return this.ctrlTextbox.CurrentLine; }
		}

		public int CodeMargin
		{
			get { return this.ctrlTextbox.CodeMargin; }
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			this.vScrollBar.Value = Math.Min(this.vScrollBar.Maximum, Math.Max(0, this.vScrollBar.Value - e.Delta / 40));
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(!this.cboSearch.Focused) {
				switch(keyData) {
					case Keys.Down:
					case Keys.Right:
						this.ctrlTextbox.CursorPosition++;
						return true;
					case Keys.Up:
					case Keys.Left:
						this.ctrlTextbox.CursorPosition--;
						return true;

					case Keys.Home:
						this.ctrlTextbox.CursorPosition = 0;
						return true;

					case Keys.End:
						this.ctrlTextbox.CursorPosition = this.ctrlTextbox.LineCount - 1;
						return true;
				}
			}

			switch(keyData) {
				case Keys.PageUp:
					this.ctrlTextbox.CursorPosition-=20;
					return true;

				case Keys.PageDown:
					this.ctrlTextbox.CursorPosition+=20;
					return true;

				case Keys.Control | Keys.F:
					this.OpenSearchBox(true);
					return true;

				case Keys.Escape:
					this.CloseSearchBox();
					return true;

				case Keys.Control | Keys.Oemplus:
					this.FontSize++;
					return true;

				case Keys.Control | Keys.OemMinus:
					this.FontSize--;
					return true;

				case Keys.Control | Keys.D0:
					this.FontSize = 13;
					return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			this.ctrlTextbox.ScrollPosition = this.vScrollBar.Value;
		}
		
		public string[] TextLines
		{
			set
			{
				this.ctrlTextbox.TextLines = value;
				this.vScrollBar.Maximum = this.ctrlTextbox.LineCount + this.vScrollBar.LargeChange;
			}
		}

		public string[] TextLineNotes
		{
			set
			{
				this.ctrlTextbox.TextLineNotes = value;
			}
		}

		public string[] CompareLines
		{
			set
			{
				this.ctrlTextbox.CompareLines = value;
			}
		}
		
		public int[] LineNumbers
		{
			set
			{
				this.ctrlTextbox.LineNumbers = value;
			}
		}

		public string[] LineNumberNotes
		{
			set
			{
				this.ctrlTextbox.LineNumberNotes = value;
			}
		}

		public bool ShowContentNotes
		{
			get { return this.ctrlTextbox.ShowContentNotes; }
			set { this.ctrlTextbox.ShowContentNotes = value; }
		}

		public bool ShowLineNumberNotes
		{
			get { return this.ctrlTextbox.ShowLineNumberNotes; }
			set { this.ctrlTextbox.ShowLineNumberNotes = value; }
		}

		public string Header
		{
			set
			{
				this.ctrlTextbox.Header = value;
			}
		}

		public int MarginWidth { set { this.ctrlTextbox.MarginWidth = value; } }

		public void OpenSearchBox(bool forceFocus = false)
		{
			bool focus = !this.panelSearch.Visible;
			this.panelSearch.Visible = true;
			if(focus || forceFocus) {
				this.cboSearch.Focus();
				this.cboSearch.SelectAll();
			}
		}

		private void CloseSearchBox()
		{
			this.ctrlTextbox.Search(null, false, false);
			this.panelSearch.Visible = false;
			this.Focus();
		}

		public void FindNext()
		{
			this.OpenSearchBox();
			this.ctrlTextbox.Search(this.cboSearch.Text, false, false);
		}

		public void FindPrevious()
		{
			this.OpenSearchBox();
			this.ctrlTextbox.Search(this.cboSearch.Text, true, false);
		}

		private void picCloseSearch_Click(object sender, EventArgs e)
		{
			this.CloseSearchBox();
		}

		private void picSearchPrevious_MouseUp(object sender, MouseEventArgs e)
		{
			this.FindPrevious();
		}

		private void picSearchNext_MouseUp(object sender, MouseEventArgs e)
		{
			this.FindNext();
		}

		private void cboSearch_TextUpdate(object sender, EventArgs e)
		{
			if(!this.ctrlTextbox.Search(this.cboSearch.Text, false, true)) {
				this.cboSearch.BackColor = Color.Coral;
			} else {
				this.cboSearch.BackColor = Color.Empty;
			}
		}

		private void cboSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter) {
				this.FindNext();
				if(this.cboSearch.Items.Contains(this.cboSearch.Text)) {
					this.cboSearch.Items.Remove(this.cboSearch.Text);
				}
				this.cboSearch.Items.Insert(0, this.cboSearch.Text);

				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		public void GoToAddress()
		{
			GoToAddress address = new GoToAddress();
			address.Address = (UInt32)this.CurrentLine;

			frmGoToLine frm = new frmGoToLine(address);
			frm.StartPosition = FormStartPosition.Manual;
			Point topLeft = this.PointToScreen(new Point(0, 0));
			frm.Location = new Point(topLeft.X + (this.Width - frm.Width) / 2, topLeft.Y + (this.Height - frm.Height) / 2);
			if(frm.ShowDialog() == DialogResult.OK) {
				this.ctrlTextbox.ScrollToLineNumber((int)address.Address);
			}
		}
	}
}
