/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2010-2011 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace OpenHardwareMonitor.GUI {
  public abstract class Gadget : IDisposable {

    private GadgetWindow window;

    public Gadget() {
      this.window = new GadgetWindow();
      this.window.Paint += delegate(object sender, PaintEventArgs e) {
        OnPaint(e);
      };
    }

    public virtual void Dispose() {
      window.Dispose();
    }

    public Point Location {
      get {
        return window.Location;
      }
      set {
        window.Location = value;
      }
    }

    public event EventHandler LocationChanged {
      add {
        window.LocationChanged += value;
      }
      remove {
        window.LocationChanged -= value;
      }
    }

    public virtual Size Size {
      get {
        return window.Size; 
      }
      set {        
        this.window.Size = value;
      }
    }

    public event EventHandler SizeChanged {
      add {
        window.SizeChanged += value;
      }
      remove {
        window.SizeChanged -= value;
      }
    }

    public byte Opacity {
      get {
        return window.Opacity;
      }
      set {
        window.Opacity = value;
      }
    }
    private byte hardwareBackgroundOpacity = 0;
    public byte HardwareBackgroundOpacity {
      get {
        return hardwareBackgroundOpacity;
      }
      set {
        hardwareBackgroundOpacity = value;
      }
    }
    private byte progressOpacity = 0;
    public byte ProgressOpacity {
      get {
        return progressOpacity;
      }
      set {
        progressOpacity = value;
      }
    }
    private byte backgroundImageOpacity = 0;
    public byte BackgroundImageOpacity
    {
      get {
        return backgroundImageOpacity;
      }
      set {
        backgroundImageOpacity = value;
      }
    }
    private byte backgroundImageCoverOpacity = 0;
    public byte BackgroundImageCoverOpacity
    {
      get {
        return backgroundImageCoverOpacity;
      }
      set {
        backgroundImageCoverOpacity = value;
      }
    }
    public bool LockPositionAndSize {
      get {
        return window.LockPositionAndSize;
      }
      set {
        window.LockPositionAndSize = value;
      }
    }

    public bool AlwaysOnTop {
      get {
        return window.AlwaysOnTop;
      }
      set {
        window.AlwaysOnTop = value;
      }
    }
    private bool backgroundImageCoverColor = false;
    public bool BackgroundImageCoverColor {
      get {
        return backgroundImageCoverColor;
      }
      set {
        backgroundImageCoverColor = value;
      }
    }

        private bool colorfulTemperature = true;
        public bool ColorfulTemperature
        {
            get
            {
                return colorfulTemperature;
            }
            set
            {
                colorfulTemperature = value;
            }
        }
        public ContextMenu ContextMenu {
      get {
        return window.ContextMenu;
      }
      set {
        window.ContextMenu = value;
      }
    }

    public event HitTestEventHandler HitTest {
      add {
        window.HitTest += value;
      }
      remove {
        window.HitTest -= value;
      }
    }

    public event MouseEventHandler MouseDoubleClick {
      add {
        window.MouseDoubleClick += value;
      }
      remove {
        window.MouseDoubleClick -= value;
      }
    }

    public bool Visible {
      get {
        return window.Visible;
      }
      set {
        if (value != window.Visible) {
          window.Visible = value;
          if (VisibleChanged != null)
            VisibleChanged(this, EventArgs.Empty);
          if (value)
            Redraw();          
        }
      }
    }

    public event EventHandler VisibleChanged;

    public void Redraw() {
      window.Redraw();
    }

    protected abstract void OnPaint(PaintEventArgs e);
  
  }
}
