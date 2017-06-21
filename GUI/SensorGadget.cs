/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2010-2012 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using OpenHardwareMonitor.Hardware;
using System.Diagnostics;

namespace OpenHardwareMonitor.GUI {
  public class SensorGadget : Gadget {

    private UnitManager unitManager;

    private Image back = Utilities.EmbeddedResources.GetImage("gadget.png");
    private Image back_bak;
    private Boolean back_isChanged;
    private Image image = null;
    private Image fore = null;
    private Image barBack = Utilities.EmbeddedResources.GetImage("barback.png");
    private Image barFore = Utilities.EmbeddedResources.GetImage("barblue.png");
    private const int topBorder = 6;
    private const int bottomBorder = 7;
    private const int leftBorder = 6;
    private const int rightBorder = 7;
    private Image background = new Bitmap(1, 1);

    private readonly float scale;
    private float fontSize;
    private int iconSize;
    private int hardwareLineHeight;
    private int sensorLineHeight;
    private int rightMargin;
    private int leftMargin;
    private int topMargin;
    private int bottomMargin;
    private int progressWidth;
    private byte[] progressColor;

    private IDictionary<IHardware, IList<ISensor>> sensors =
      new SortedDictionary<IHardware, IList<ISensor>>(new HardwareComparer());

    private PersistentSettings settings;
    private UserOption hardwareNames;
    private UserOption alwaysOnTop;
    private UserOption lockPositionAndSize;
    private UserOption backgroundImageCoverColor;

    private Font largeFont;
    private Font smallFont;
    private Brush darkWhite;
    private StringFormat stringFormat;
    private StringFormat trimStringFormat;
    private StringFormat alignRightStringFormat;

    public SensorGadget(IComputer computer, PersistentSettings settings, 
      UnitManager unitManager) 
    {
      this.unitManager = unitManager;
      this.settings = settings;
      computer.HardwareAdded += new HardwareEventHandler(HardwareAdded);
      computer.HardwareRemoved += new HardwareEventHandler(HardwareRemoved);      

      this.darkWhite = new SolidBrush(Color.FromArgb(0xF0, 0xF0, 0xF0));

      this.stringFormat = new StringFormat();
      this.stringFormat.FormatFlags = StringFormatFlags.NoWrap;

      this.trimStringFormat = new StringFormat();
      this.trimStringFormat.Trimming = StringTrimming.EllipsisCharacter;
      this.trimStringFormat.FormatFlags = StringFormatFlags.NoWrap;

      this.alignRightStringFormat = new StringFormat();
      this.alignRightStringFormat.Alignment = StringAlignment.Far;
      this.alignRightStringFormat.FormatFlags = StringFormatFlags.NoWrap;
      progressColor = new byte[] { 51, 153, 51, 51, 153, 51, 51, 153, 51, 51, 153, 51, 52, 153, 51, 52, 153, 51, 52, 153, 51, 52, 153, 51, 53, 153, 51, 53, 153, 51, 53, 153, 51, 53, 153, 51, 53, 153, 51, 53, 153, 51, 53, 153, 51, 53, 153, 51, 54, 154, 51, 54, 154, 51, 54, 154, 51, 54, 154, 51, 55, 154, 51, 55, 154, 51, 55, 154, 51, 55, 154, 51, 56, 154, 51, 56, 154, 51, 56, 154, 51, 57, 154, 51, 57, 154, 51, 57, 154, 51, 57, 154, 51, 58, 155, 51, 58, 155, 51, 58, 155, 51, 58, 155, 51, 59, 155, 51, 59, 155, 51, 59, 155, 51, 59, 155, 51, 60, 155, 51, 60, 155, 51, 60, 155, 51, 60, 155, 51, 61, 155, 51, 61, 155, 51, 61, 155, 51, 61, 155, 51, 62, 155, 51, 62, 155, 51, 62, 155, 51, 62, 155, 51, 63, 156, 51, 63, 156, 51, 63, 156, 51, 63, 156, 51, 64, 156, 51, 64, 156, 51, 64, 156, 51, 64, 156, 51, 65, 156, 51, 65, 156, 51, 65, 156, 51, 65, 156, 51, 66, 156, 51, 66, 156, 51, 66, 156, 51, 66, 156, 51, 67, 157, 51, 67, 157, 51, 67, 157, 51, 67, 157, 51, 68, 157, 51, 68, 157, 51, 68, 157, 51, 68, 157, 51, 68, 157, 51, 68, 157, 51, 68, 157, 51, 69, 157, 51, 69, 157, 51, 69, 157, 51, 69, 157, 51, 70, 158, 51, 70, 158, 51, 70, 158, 51, 70, 158, 51, 71, 158, 51, 71, 158, 51, 71, 158, 51, 71, 158, 51, 72, 158, 51, 72, 158, 51, 72, 158, 51, 72, 158, 51, 73, 158, 51, 73, 158, 51, 73, 158, 51, 73, 158, 51, 74, 159, 51, 74, 159, 51, 74, 159, 51, 74, 159, 51, 75, 159, 51, 75, 159, 51, 75, 159, 51, 75, 159, 51, 76, 159, 51, 76, 159, 51, 76, 159, 51, 76, 159, 51, 77, 159, 51, 77, 159, 51, 77, 159, 51, 77, 159, 51, 78, 159, 51, 78, 159, 51, 78, 159, 51, 79, 159, 51, 79, 160, 51, 79, 160, 51, 79, 160, 51, 80, 160, 51, 80, 160, 51, 80, 160, 51, 80, 160, 51, 81, 160, 51, 81, 160, 51, 81, 160, 51, 81, 160, 51, 82, 160, 51, 82, 160, 51, 82, 160, 51, 82, 160, 51, 83, 161, 51, 83, 161, 51, 83, 161, 51, 83, 161, 51, 84, 161, 51, 84, 161, 51, 84, 161, 51, 84, 161, 51, 84, 161, 51, 84, 161, 51, 84, 161, 51, 84, 161, 51, 85, 161, 51, 85, 161, 51, 85, 161, 51, 85, 161, 51, 86, 162, 51, 86, 162, 51, 86, 162, 51, 86, 162, 51, 87, 162, 51, 87, 162, 51, 87, 162, 51, 87, 162, 51, 88, 162, 51, 88, 162, 51, 88, 162, 51, 88, 162, 51, 89, 162, 51, 89, 162, 51, 89, 162, 51, 90, 162, 51, 90, 162, 51, 90, 162, 51, 90, 162, 51, 91, 163, 51, 91, 163, 51, 91, 163, 51, 91, 163, 51, 92, 163, 51, 92, 163, 51, 92, 163, 51, 92, 163, 51, 93, 163, 51, 93, 163, 51, 93, 163, 51, 93, 163, 51, 94, 163, 51, 94, 163, 51, 94, 163, 51, 94, 163, 51, 95, 164, 51, 95, 164, 51, 95, 164, 51, 95, 164, 51, 96, 164, 51, 96, 164, 51, 96, 164, 51, 96, 164, 51, 97, 164, 51, 97, 164, 51, 97, 164, 51, 97, 164, 51, 98, 164, 51, 98, 164, 51, 98, 164, 51, 98, 164, 51, 99, 165, 51, 99, 165, 51, 99, 165, 51, 99, 165, 51, 100, 165, 51, 100, 165, 51, 100, 165, 51, 100, 165, 51, 100, 165, 51, 100, 165, 51, 100, 165, 51, 100, 165, 51, 101, 165, 51, 101, 165, 51, 101, 165, 51, 102, 166, 51, 102, 166, 51, 102, 166, 51, 102, 166, 51, 103, 166, 51, 103, 166, 51, 103, 166, 51, 103, 166, 51, 104, 166, 51, 104, 166, 51, 104, 166, 51, 104, 166, 51, 105, 166, 51, 105, 166, 51, 105, 166, 51, 105, 166, 51, 106, 166, 51, 106, 166, 51, 106, 166, 51, 106, 166, 51, 107, 167, 51, 107, 167, 51, 107, 167, 51, 107, 167, 51, 108, 167, 51, 108, 167, 51, 108, 167, 51, 108, 167, 51, 109, 167, 51, 109, 167, 51, 109, 167, 51, 109, 167, 51, 110, 167, 51, 110, 167, 51, 110, 167, 51, 110, 167, 51, 111, 168, 51, 111, 168, 51, 111, 168, 51, 112, 168, 51, 112, 168, 51, 112, 168, 51, 112, 168, 51, 112, 168, 51, 113, 168, 51, 113, 168, 51, 113, 168, 51, 114, 168, 51, 114, 168, 51, 114, 168, 51, 114, 168, 51, 115, 169, 51, 115, 169, 51, 115, 169, 51, 115, 169, 51, 115, 169, 51, 115, 169, 51, 115, 169, 51, 115, 169, 51, 116, 169, 51, 116, 169, 51, 116, 169, 51, 116, 169, 51, 117, 169, 51, 117, 169, 51, 117, 169, 51, 117, 169, 51, 118, 170, 51, 118, 170, 51, 118, 170, 51, 118, 170, 51, 119, 170, 51, 119, 170, 51, 119, 170, 51, 119, 170, 51, 120, 170, 51, 120, 170, 51, 120, 170, 51, 120, 170, 51, 121, 170, 51, 121, 170, 51, 121, 170, 51, 121, 170, 51, 122, 170, 51, 122, 170, 51, 122, 170, 51, 123, 171, 51, 123, 171, 51, 123, 171, 51, 123, 171, 51, 124, 171, 51, 124, 171, 51, 124, 171, 51, 124, 171, 51, 125, 171, 51, 125, 171, 51, 125, 171, 51, 125, 171, 51, 126, 171, 51, 126, 171, 51, 126, 171, 51, 126, 171, 51, 127, 172, 51, 127, 172, 51, 127, 172, 51, 127, 172, 51, 128, 172, 51, 128, 172, 51, 128, 172, 51, 128, 172, 51, 129, 172, 51, 129, 172, 51, 129, 172, 51, 129, 172, 51, 130, 172, 51, 130, 172, 51, 130, 172, 51, 130, 172, 51, 131, 173, 51, 131, 173, 51, 131, 173, 51, 131, 173, 51, 131, 173, 51, 131, 173, 51, 131, 173, 51, 131, 173, 51, 132, 173, 51, 132, 173, 51, 132, 173, 51, 132, 173, 51, 133, 173, 51, 133, 173, 51, 133, 173, 51, 134, 174, 51, 134, 174, 51, 134, 174, 51, 134, 174, 51, 135, 174, 51, 135, 174, 51, 135, 174, 51, 135, 174, 51, 136, 174, 51, 136, 174, 51, 136, 174, 51, 136, 174, 51, 137, 174, 51, 137, 174, 51, 137, 174, 51, 137, 174, 51, 138, 174, 51, 138, 174, 51, 138, 174, 51, 138, 174, 51, 139, 175, 51, 139, 175, 51, 139, 175, 51, 139, 175, 51, 140, 175, 51, 140, 175, 51, 140, 175, 51, 140, 175, 51, 141, 175, 51, 141, 175, 51, 141, 175, 51, 141, 175, 51, 142, 175, 51, 142, 175, 51, 142, 175, 51, 142, 175, 51, 143, 176, 51, 143, 176, 51, 143, 176, 51, 143, 176, 51, 144, 176, 51, 144, 176, 51, 144, 176, 51, 144, 176, 51, 145, 176, 51, 145, 176, 51, 145, 176, 51, 145, 176, 51, 146, 176, 51, 146, 176, 51, 146, 176, 51, 147, 177, 51, 147, 177, 51, 147, 177, 51, 147, 177, 51, 147, 177, 51, 147, 177, 51, 147, 177, 51, 147, 177, 51, 148, 177, 51, 148, 177, 51, 148, 177, 51, 148, 177, 51, 149, 177, 51, 149, 177, 51, 149, 177, 51, 149, 177, 51, 150, 178, 51, 150, 178, 51, 150, 178, 51, 150, 178, 51, 151, 178, 51, 151, 178, 51, 151, 178, 51, 151, 178, 51, 152, 178, 51, 152, 178, 51, 152, 178, 51, 152, 178, 51, 153, 178, 51, 153, 178, 51, 153, 178, 51, 153, 178, 51, 154, 178, 51, 154, 178, 51, 154, 178, 51, 154, 178, 51, 155, 179, 51, 155, 179, 51, 155, 179, 51, 156, 179, 51, 156, 179, 51, 156, 179, 51, 156, 179, 51, 157, 179, 51, 157, 179, 51, 157, 179, 51, 157, 179, 51, 158, 179, 51, 158, 179, 51, 158, 179, 51, 158, 179, 51, 159, 180, 51, 159, 180, 51, 159, 180, 51, 159, 180, 51, 160, 180, 51, 160, 180, 51, 160, 180, 51, 160, 180, 51, 161, 180, 51, 161, 180, 51, 161, 180, 51, 161, 180, 51, 162, 180, 51, 162, 180, 51, 162, 180, 51, 162, 180, 51, 162, 181, 51, 162, 181, 51, 162, 181, 51, 162, 181, 51, 163, 181, 51, 163, 181, 51, 163, 181, 51, 163, 181, 51, 164, 181, 51, 164, 181, 51, 164, 181, 51, 164, 181, 51, 165, 181, 51, 165, 181, 51, 165, 181, 51, 165, 181, 51, 166, 182, 51, 166, 182, 51, 166, 182, 51, 167, 182, 51, 167, 182, 51, 167, 182, 51, 167, 182, 51, 168, 182, 51, 168, 182, 51, 168, 182, 51, 168, 182, 51, 169, 182, 51, 169, 182, 51, 169, 182, 51, 169, 182, 51, 170, 182, 51, 170, 182, 51, 170, 182, 51, 170, 182, 51, 171, 183, 51, 171, 183, 51, 171, 183, 51, 171, 183, 51, 172, 183, 51, 172, 183, 51, 172, 183, 51, 172, 183, 51, 173, 183, 51, 173, 183, 51, 173, 183, 51, 173, 183, 51, 174, 183, 51, 174, 183, 51, 174, 183, 51, 174, 183, 51, 175, 184, 51, 175, 184, 51, 175, 184, 51, 175, 184, 51, 176, 184, 51, 176, 184, 51, 176, 184, 51, 176, 184, 51, 177, 184, 51, 177, 184, 51, 177, 184, 51, 177, 184, 51, 178, 184, 51, 178, 184, 51, 178, 184, 51, 178, 185, 51, 178, 185, 51, 178, 185, 51, 178, 185, 51, 179, 185, 51, 179, 185, 51, 179, 185, 51, 179, 185, 51, 180, 185, 51, 180, 185, 51, 180, 185, 51, 180, 185, 51, 181, 185, 51, 181, 185, 51, 181, 185, 51, 181, 185, 51, 182, 186, 51, 182, 186, 51, 182, 186, 51, 182, 186, 51, 183, 186, 51, 183, 186, 51, 183, 186, 51, 183, 186, 51, 184, 186, 51, 184, 186, 51, 184, 186, 51, 184, 186, 51, 185, 186, 51, 185, 186, 51, 185, 186, 51, 185, 186, 51, 186, 186, 51, 186, 186, 51, 186, 186, 51, 186, 186, 51, 187, 187, 51, 187, 187, 51, 187, 187, 51, 187, 187, 51, 188, 187, 51, 188, 187, 51, 188, 187, 51, 189, 187, 51, 189, 187, 51, 189, 187, 51, 189, 187, 51, 190, 187, 51, 190, 187, 51, 190, 187, 51, 190, 187, 51, 191, 188, 51, 191, 188, 51, 191, 188, 51, 191, 188, 51, 192, 188, 51, 192, 188, 51, 192, 188, 51, 192, 188, 51, 193, 188, 51, 193, 188, 51, 193, 188, 51, 193, 188, 51, 194, 188, 51, 194, 188, 51, 194, 188, 51, 194, 188, 51, 194, 189, 51, 194, 189, 51, 194, 189, 51, 194, 189, 51, 195, 189, 51, 195, 189, 51, 195, 189, 51, 195, 189, 51, 196, 189, 51, 196, 189, 51, 196, 189, 51, 196, 189, 51, 197, 189, 51, 197, 189, 51, 197, 189, 51, 197, 189, 51, 198, 190, 51, 198, 190, 51, 198, 190, 51, 198, 190, 51, 199, 190, 51, 199, 190, 51, 199, 190, 51, 200, 190, 51, 200, 190, 51, 200, 190, 51, 200, 190, 51, 201, 190, 51, 201, 190, 51, 201, 190, 51, 201, 190, 51, 202, 190, 51, 202, 190, 51, 202, 190, 51, 202, 190, 51, 203, 191, 51, 203, 191, 51, 203, 191, 51, 203, 191, 51, 204, 191, 51, 204, 191, 51, 204, 191, 51, 204, 191, 51, 205, 191, 51, 205, 191, 51, 205, 191, 51, 205, 191, 51, 206, 191, 51, 206, 191, 51, 206, 191, 51, 206, 191, 51, 207, 192, 51, 207, 192, 51, 207, 192, 51, 207, 192, 51, 208, 192, 51, 208, 192, 51, 208, 192, 51, 208, 192, 51, 209, 192, 51, 209, 192, 51, 209, 192, 51, 209, 192, 51, 209, 192, 51, 209, 192, 51, 209, 192, 51, 209, 192, 51, 210, 193, 51, 210, 193, 51, 210, 193, 51, 211, 193, 51, 211, 193, 51, 211, 193, 51, 211, 193, 51, 212, 193, 51, 212, 193, 51, 212, 193, 51, 212, 193, 51, 213, 193, 51, 213, 193, 51, 213, 193, 51, 213, 193, 51, 214, 194, 51, 214, 194, 51, 214, 194, 51, 214, 194, 51, 215, 194, 51, 215, 194, 51, 215, 194, 51, 215, 194, 51, 216, 194, 51, 216, 194, 51, 216, 194, 51, 216, 194, 51, 217, 194, 51, 217, 194, 51, 217, 194, 51, 217, 194, 51, 218, 194, 51, 218, 194, 51, 218, 194, 51, 218, 194, 51, 219, 195, 51, 219, 195, 51, 219, 195, 51, 219, 195, 51, 220, 195, 51, 220, 195, 51, 220, 195, 51, 221, 195, 51, 221, 195, 51, 221, 195, 51, 221, 195, 51, 222, 195, 51, 222, 195, 51, 222, 195, 51, 222, 195, 51, 223, 196, 51, 223, 196, 51, 223, 196, 51, 223, 196, 51, 224, 196, 51, 224, 196, 51, 224, 196, 51, 224, 196, 51, 225, 196, 51, 225, 196, 51, 225, 196, 51, 225, 196, 51, 225, 196, 51, 225, 196, 51, 225, 196, 51, 225, 196, 51, 226, 197, 51, 226, 197, 51, 226, 197, 51, 226, 197, 51, 227, 197, 51, 227, 197, 51, 227, 197, 51, 227, 197, 51, 228, 197, 51, 228, 197, 51, 228, 197, 51, 228, 197, 51, 229, 197, 51, 229, 197, 51, 229, 197, 51, 229, 197, 51, 230, 198, 51, 230, 198, 51, 230, 198, 51, 230, 198, 51, 231, 198, 51, 231, 198, 51, 231, 198, 51, 231, 198, 51, 232, 198, 51, 232, 198, 51, 232, 198, 51, 233, 198, 51, 233, 198, 51, 233, 198, 51, 233, 198, 51, 234, 198, 51, 234, 198, 51, 234, 198, 51, 234, 198, 51, 235, 199, 51, 235, 199, 51, 235, 199, 51, 235, 199, 51, 236, 199, 51, 236, 199, 51, 236, 199, 51, 236, 199, 51, 237, 199, 51, 237, 199, 51, 237, 199, 51, 237, 199, 51, 238, 199, 51, 238, 199, 51, 238, 199, 51, 238, 199, 51, 239, 200, 51, 239, 200, 51, 239, 200, 51, 239, 200, 51, 240, 200, 51, 240, 200, 51, 240, 200, 51, 240, 200, 51, 241, 200, 51, 241, 200, 51, 241, 200, 51, 241, 200, 51, 241, 200, 51, 241, 200, 51, 241, 200, 51, 241, 200, 51, 242, 201, 51, 242, 201, 51, 242, 201, 51, 242, 201, 51, 243, 201, 51, 243, 201, 51, 243, 201, 51, 244, 201, 51, 244, 201, 51, 244, 201, 51, 244, 201, 51, 245, 201, 51, 245, 201, 51, 245, 201, 51, 245, 201, 51, 246, 202, 51, 246, 202, 51, 246, 202, 51, 246, 202, 51, 247, 202, 51, 247, 202, 51, 247, 202, 51, 247, 202, 51, 248, 202, 51, 248, 202, 51, 248, 202, 51, 248, 202, 51, 249, 202, 51, 249, 202, 51, 249, 202, 51, 249, 202, 51, 250, 202, 51, 250, 202, 51, 250, 202, 51, 250, 202, 51, 251, 203, 51, 251, 203, 51, 251, 203, 51, 251, 203, 51, 252, 203, 51, 252, 203, 51, 252, 203, 51, 252, 203, 51, 253, 203, 51, 253, 203, 51, 253, 203, 51, 254, 203, 51, 254, 203, 51, 254, 203, 51, 254, 203, 51, 255, 204, 51, 255, 204, 51, 255, 204, 51, 255, 203, 50, 254, 202, 50, 254, 202, 49, 254, 201, 49, 254, 200, 48, 253, 200, 48, 253, 199, 47, 253, 199, 47, 253, 198, 47, 252, 198, 46, 252, 197, 46, 252, 197, 46, 252, 196, 45, 251, 196, 45, 251, 195, 44, 251, 195, 44, 251, 194, 44, 250, 194, 43, 250, 193, 43, 250, 193, 43, 250, 192, 42, 249, 192, 42, 249, 191, 41, 249, 191, 41, 249, 190, 40, 248, 190, 40, 248, 189, 39, 248, 188, 39, 248, 188, 39, 247, 187, 38, 247, 186, 38, 247, 186, 38, 246, 185, 37, 246, 185, 36, 246, 184, 36, 246, 183, 35, 245, 183, 35, 245, 182, 34, 245, 182, 34, 245, 181, 34, 244, 181, 33, 244, 180, 33, 244, 180, 33, 244, 179, 32, 243, 179, 32, 243, 178, 31, 243, 178, 31, 243, 177, 31, 242, 177, 30, 242, 176, 30, 242, 176, 30, 242, 174, 30, 242, 173, 29, 242, 172, 29, 242, 170, 29, 242, 169, 29, 242, 168, 28, 242, 167, 28, 242, 165, 28, 242, 164, 28, 242, 163, 28, 242, 161, 28, 242, 160, 28, 242, 159, 28, 242, 158, 27, 242, 157, 27, 242, 156, 27, 242, 155, 27, 242, 154, 27, 242, 152, 27, 242, 151, 27, 242, 150, 27, 243, 148, 26, 243, 147, 26, 243, 146, 26, 243, 145, 26, 243, 144, 25, 243, 143, 25, 243, 142, 25, 243, 141, 25, 243, 139, 25, 243, 138, 25, 243, 137, 25, 243, 135, 24, 243, 134, 24, 243, 133, 24, 243, 132, 24, 243, 130, 24, 243, 129, 24, 243, 128, 24, 243, 127, 24, 243, 126, 23, 243, 125, 23, 243, 124, 23, 243, 122, 23, 244, 121, 22, 244, 120, 22, 244, 119, 22, 244, 117, 22, 244, 116, 22, 244, 115, 22, 244, 114, 22, 244, 112, 22, 244, 112, 21, 244, 110, 21, 244, 109, 21, 244, 108, 21, 244, 107, 21, 244, 105, 21, 244, 104, 21, 244, 103, 21, 244, 102, 20, 244, 100, 20, 244, 99, 20, 244, 98, 20, 244, 97, 19, 244, 95, 19, 244, 94, 19, 244, 93, 19, 245, 92, 19, 245, 91, 19, 245, 90, 19, 245, 89, 18, 245, 87, 18, 245, 86, 18, 245, 85, 18, 245, 83, 18, 245, 82, 18, 245, 81, 18, 245, 80, 18, 245, 79, 17, 245, 78, 17, 245, 77, 17, 245, 76, 17, 245, 74, 16, 245, 73, 16, 245, 72, 16, 245, 70, 16, 245, 69, 16, 245, 68, 16, 245, 67, 16, 245, 65, 16, 245, 64, 15, 245, 63, 15, 245, 62, 15, 245, 61, 15, 246, 60, 15, 246, 59, 15, 246, 58, 15 };
      if (File.Exists("gadget_background.png")) {
        try { 
          Image newBack = new Bitmap("gadget_background.png");
          back.Dispose();
          back = newBack;
        } catch { }
      }
      back_bak = back;
      back = TransparentImage(back_bak, (float)settings.GetValue("sensorGadget.BackgroundImageOpacity", BackgroundImageOpacity) / 255);

      if (File.Exists("gadget_image.png")) {
        try {
          image = new Bitmap("gadget_image.png"); 
        } catch {}
      }

      if (File.Exists("gadget_foreground.png")) {
        try {
          fore = new Bitmap("gadget_foreground.png"); 
        } catch { }
      }

      if (File.Exists("gadget_bar_background.png")) {
        try {
          Image newBarBack = new Bitmap("gadget_bar_background.png");
          barBack.Dispose();
          barBack = newBarBack;
        } catch { }
      }

      if (File.Exists("gadget_bar_foreground.png")) {
        try {
          Image newBarColor = new Bitmap("gadget_bar_foreground.png");
          barFore.Dispose();
          barFore = newBarColor;
        } catch { }
      }

      this.Location = new Point(
        settings.GetValue("sensorGadget.Location.X", 100),
        settings.GetValue("sensorGadget.Location.Y", 100)); 
      LocationChanged += delegate(object sender, EventArgs e) {
        settings.SetValue("sensorGadget.Location.X", Location.X);
        settings.SetValue("sensorGadget.Location.Y", Location.Y);
      };

      // get the custom to default dpi ratio
      using (Bitmap b = new Bitmap(1, 1)) {
        scale = b.HorizontalResolution / 96.0f;
      }

      SetFontSize(settings.GetValue("sensorGadget.FontSize", 7.5f));
      Resize(settings.GetValue("sensorGadget.Width", Size.Width));
      
      ContextMenu contextMenu = new ContextMenu();
      MenuItem hardwareNamesItem = new MenuItem("硬件名称");
      contextMenu.MenuItems.Add(hardwareNamesItem);
      MenuItem fontSizeMenu = new MenuItem("字体大小");
      for (int i = 0; i < 4; i++) {
        float size;
        string name;
        switch (i) {
          case 0: size = 6.5f; name = "小"; break;
          case 1: size = 7.5f; name = "中"; break;
          case 2: size = 9f; name = "大"; break;
          case 3: size = 11f; name = "夶"; break;
          default: throw new NotImplementedException();
        }
        MenuItem item = new MenuItem(name);
        item.Checked = fontSize == size;
        item.Click += delegate(object sender, EventArgs e) {
          SetFontSize(size);
          settings.SetValue("sensorGadget.FontSize", size);
          foreach (MenuItem mi in fontSizeMenu.MenuItems)
            mi.Checked = mi == item;
        };
        fontSizeMenu.MenuItems.Add(item);
      }
      contextMenu.MenuItems.Add(fontSizeMenu);
      contextMenu.MenuItems.Add(new MenuItem("-"));
      MenuItem lockItem = new MenuItem("锁定位置和大小");
      contextMenu.MenuItems.Add(lockItem);
      MenuItem alwaysOnTopItem = new MenuItem("保持前端显示");
      contextMenu.MenuItems.Add(alwaysOnTopItem);
      contextMenu.MenuItems.Add(new MenuItem("-"));
      MenuItem opacityMenu = new MenuItem("总不透明度");
      contextMenu.MenuItems.Add(opacityMenu);
      Opacity = (byte)settings.GetValue("sensorGadget.Opacity", 255);      
      for (int i = 0; i < 5; i++) {
        MenuItem item = new MenuItem((20 * (i + 1)).ToString() + " %");
        byte o = (byte)(51 * (i + 1));
        item.Checked = Opacity == o;
        item.Click += delegate(object sender, EventArgs e) {
          Opacity = o;
          settings.SetValue("sensorGadget.Opacity", Opacity);
          foreach (MenuItem mi in opacityMenu.MenuItems)
            mi.Checked = mi == item;          
        };
        opacityMenu.MenuItems.Add(item);
      }
      MenuItem backgroundImageOpacityMenu = new MenuItem("背景图片不透明度");
      contextMenu.MenuItems.Add(backgroundImageOpacityMenu);
      BackgroundImageOpacity = (byte)settings.GetValue("sensorGadget.BackgroundImageOpacity", 255);      
      for (int i = -1; i < 5; i++) {
        MenuItem item = new MenuItem((20 * (i + 1)).ToString() + " %");
        byte o = (byte)(51 * (i + 1));
        item.Checked = BackgroundImageOpacity == o;
        item.Click += delegate(object sender, EventArgs e) {
            BackgroundImageOpacity = o;
          settings.SetValue("sensorGadget.BackgroundImageOpacity", BackgroundImageOpacity);
          foreach (MenuItem mi in backgroundImageOpacityMenu.MenuItems)
            mi.Checked = mi == item;
          back.Dispose();
          back = TransparentImage(back_bak, (float)BackgroundImageOpacity/255);
          back_isChanged = true;
        };
        backgroundImageOpacityMenu.MenuItems.Add(item);
      }
      MenuItem backgroundImageCoverOpacityMenu = new MenuItem("背景图片蒙层不透明度");
      contextMenu.MenuItems.Add(backgroundImageCoverOpacityMenu);
      BackgroundImageCoverOpacity = (byte)settings.GetValue("sensorGadget.BackgroundImageCoverOpacity", 255);      
      for (int i = -1; i < 5; i++) {
        MenuItem item = new MenuItem((20 * (i + 1)).ToString() + " %");
        byte o = (byte)(51 * (i + 1));
        item.Checked = BackgroundImageCoverOpacity == o;
        item.Click += delegate(object sender, EventArgs e) {
            BackgroundImageCoverOpacity = o;
          settings.SetValue("sensorGadget.BackgroundImageCoverOpacity", BackgroundImageCoverOpacity);
          foreach (MenuItem mi in backgroundImageCoverOpacityMenu.MenuItems)
            mi.Checked = mi == item;
        };
        backgroundImageCoverOpacityMenu.MenuItems.Add(item);
      }
      MenuItem backgroundImageCoverColorItem = new MenuItem("背景图片蒙层颜色(黑/白)");
      contextMenu.MenuItems.Add(backgroundImageCoverColorItem);
      this.ContextMenu = contextMenu;

      hardwareNames = new UserOption("sensorGadget.Hardwarenames", true,
        hardwareNamesItem, settings);
      hardwareNames.Changed += delegate(object sender, EventArgs e) {
        Resize();
      };
      MenuItem hardwareBackgroundOpacityMenu = new MenuItem("硬件栏不透明度");
      contextMenu.MenuItems.Add(hardwareBackgroundOpacityMenu);
      HardwareBackgroundOpacity = (byte)settings.GetValue("sensorGadget.HardwareBackgroundOpacity", 255);      
      for (int i = -1; i < 5; i++) {
        MenuItem item = new MenuItem((20 * (i + 1)).ToString() + " %");
        byte o = (byte)(51 * (i + 1));
        item.Checked = HardwareBackgroundOpacity == o;
        item.Click += delegate(object sender, EventArgs e) {
          HardwareBackgroundOpacity = o;
          settings.SetValue("sensorGadget.HardwareBackgroundOpacity", HardwareBackgroundOpacity);
          foreach (MenuItem mi in hardwareBackgroundOpacityMenu.MenuItems)
            mi.Checked = mi == item;          
        };
        hardwareBackgroundOpacityMenu.MenuItems.Add(item);
      }
      MenuItem progressOpacityMenu = new MenuItem("进度条不透明度");
      contextMenu.MenuItems.Add(progressOpacityMenu);
      ProgressOpacity = (byte)settings.GetValue("sensorGadget.ProgressOpacityy", 255);      
      for (int i = -1; i < 5; i++) {
        MenuItem item = new MenuItem((20 * (i + 1)).ToString() + " %");
        byte o = (byte)(51 * (i + 1));
        item.Checked = ProgressOpacity == o;
        item.Click += delegate(object sender, EventArgs e) {
          ProgressOpacity = o;
          settings.SetValue("sensorGadget.ProgressOpacity", ProgressOpacity);
          foreach (MenuItem mi in progressOpacityMenu.MenuItems)
            mi.Checked = mi == item;          
        };
        progressOpacityMenu.MenuItems.Add(item);
      }

      alwaysOnTop = new UserOption("sensorGadget.AlwaysOnTop", false, 
        alwaysOnTopItem, settings);
      alwaysOnTop.Changed += delegate(object sender, EventArgs e) {
        this.AlwaysOnTop = alwaysOnTop.Value;
      };
      lockPositionAndSize = new UserOption("sensorGadget.LockPositionAndSize", 
        false, lockItem, settings);
      lockPositionAndSize.Changed += delegate(object sender, EventArgs e) {
        this.LockPositionAndSize = lockPositionAndSize.Value;
      };
      backgroundImageCoverColor = new UserOption("sensorGadget.BackgroundImageCoverColor", false,
        backgroundImageCoverColorItem, settings);
      backgroundImageCoverColor.Changed += delegate(object sender, EventArgs e) {
        this.BackgroundImageCoverColor = backgroundImageCoverColor.Value;
      };

      HitTest += delegate(object sender, HitTestEventArgs e) {
        if (lockPositionAndSize.Value)
          return;

        if (e.Location.X < leftBorder) {
          e.HitResult = HitResult.Left;
          return;
        }
        if (e.Location.X > Size.Width - 1 - rightBorder) {
          e.HitResult = HitResult.Right;
          return;
        }
      };

      SizeChanged += delegate(object sender, EventArgs e) {
        settings.SetValue("sensorGadget.Width", Size.Width);
        Redraw();
      };

      VisibleChanged += delegate(object sender, EventArgs e) {
        Rectangle bounds = new Rectangle(Location, Size);
        Screen screen = Screen.FromRectangle(bounds);
        Rectangle intersection = 
          Rectangle.Intersect(screen.WorkingArea, bounds);
        if (intersection.Width < Math.Min(16, bounds.Width) || 
            intersection.Height < Math.Min(16, bounds.Height)) 
        {
          Location = new Point(
            screen.WorkingArea.Width / 2 - bounds.Width / 2, 
            screen.WorkingArea.Height / 2 - bounds.Height / 2);
        }
      };

      MouseDoubleClick += delegate(object obj, MouseEventArgs args) {
        SendHideShowCommand();
      };
    }

    public override void Dispose() {

      largeFont.Dispose();
      largeFont = null;

      smallFont.Dispose();
      smallFont = null;

      darkWhite.Dispose();
      darkWhite = null;

      stringFormat.Dispose();
      stringFormat = null;

      trimStringFormat.Dispose();
      trimStringFormat = null;

      alignRightStringFormat.Dispose();
      alignRightStringFormat = null;     
 
      back.Dispose();
      back = null;

      barFore.Dispose();
      barFore = null;

      barBack.Dispose();
      barBack = null;

      background.Dispose();
      background = null;

      if (image != null) {
        image.Dispose();
        image = null;
      }

      if (fore != null) {
        fore.Dispose();
        fore = null;
      }

      base.Dispose();
    }

    private void HardwareRemoved(IHardware hardware) {
      hardware.SensorAdded -= new SensorEventHandler(SensorAdded);
      hardware.SensorRemoved -= new SensorEventHandler(SensorRemoved);
      foreach (ISensor sensor in hardware.Sensors)
        SensorRemoved(sensor);
      foreach (IHardware subHardware in hardware.SubHardware)
        HardwareRemoved(subHardware);
    }

    private void HardwareAdded(IHardware hardware) {
      foreach (ISensor sensor in hardware.Sensors)
        SensorAdded(sensor);
      hardware.SensorAdded += new SensorEventHandler(SensorAdded);
      hardware.SensorRemoved += new SensorEventHandler(SensorRemoved);
      foreach (IHardware subHardware in hardware.SubHardware)
        HardwareAdded(subHardware);
    }

    private void SensorAdded(ISensor sensor) {
      if (settings.GetValue(new Identifier(sensor.Identifier,
        "gadget").ToString(), false)) 
        Add(sensor);
    }

    private void SensorRemoved(ISensor sensor) {
      if (Contains(sensor))
        Remove(sensor, false);
    }

    public bool Contains(ISensor sensor) {
      foreach (IList<ISensor> list in sensors.Values)
        if (list.Contains(sensor))
          return true;
      return false;
    }

    public void Add(ISensor sensor) {
      if (Contains(sensor)) {
        return;
      } else {
        // get the right hardware
        IHardware hardware = sensor.Hardware;
        while (hardware.Parent != null)
          hardware = hardware.Parent;

        // get the sensor list associated with the hardware
        IList<ISensor> list;
        if (!sensors.TryGetValue(hardware, out list)) {
          list = new List<ISensor>();
          sensors.Add(hardware, list);
        }

        // insert the sensor at the right position
        int i = 0;
        while (i < list.Count && (list[i].SensorType < sensor.SensorType || 
          (list[i].SensorType == sensor.SensorType && 
           list[i].Index < sensor.Index))) i++;
        list.Insert(i, sensor);

        settings.SetValue(
          new Identifier(sensor.Identifier, "gadget").ToString(), true);
        
        Resize();
      }
    }

    public void Remove(ISensor sensor) {
      Remove(sensor, true);
    }

    private void Remove(ISensor sensor, bool deleteConfig) {
      if (deleteConfig) 
        settings.Remove(new Identifier(sensor.Identifier, "gadget").ToString());

      foreach (KeyValuePair<IHardware, IList<ISensor>> keyValue in sensors)
        if (keyValue.Value.Contains(sensor)) {
          keyValue.Value.Remove(sensor);          
          if (keyValue.Value.Count == 0) {
            sensors.Remove(keyValue.Key);
            break;
          }
        }
      Resize();
    }

    public event EventHandler HideShowCommand;

    public void SendHideShowCommand() {
      if (HideShowCommand != null)
        HideShowCommand(this, null);
    }

    private Font CreateFont(float size, FontStyle style) {
      try {
        return new Font(SystemFonts.MessageBoxFont.FontFamily, size, style);
      } catch (ArgumentException) {
        // if the style is not supported, fall back to the original one
        return new Font(SystemFonts.MessageBoxFont.FontFamily, size, 
          SystemFonts.MessageBoxFont.Style);
      }
    }

    private void SetFontSize(float size) {
      fontSize = size;
      largeFont = CreateFont(fontSize, FontStyle.Bold);
      smallFont = CreateFont(fontSize, FontStyle.Regular);
      
      double scaledFontSize = fontSize * scale;
      iconSize = (int)Math.Round(1.5 * scaledFontSize);
      hardwareLineHeight = (int)Math.Round(2.25 * scaledFontSize);
      sensorLineHeight = (int)Math.Round(1.8 * scaledFontSize);
      leftMargin = leftBorder + (int)Math.Round(0.3 * scaledFontSize);
      rightMargin = rightBorder + (int)Math.Round(0.3 * scaledFontSize);
      topMargin = topBorder;
      bottomMargin = bottomBorder + (int)Math.Round(0.3 * scaledFontSize);
      progressWidth = (int)Math.Round(5.3 * scaledFontSize);

      Resize((int)Math.Round(17.3 * scaledFontSize));
    }

    private void Resize() {
      Resize(this.Size.Width);
    }

    private void Resize(int width) {
      int y = topMargin;      
      foreach (KeyValuePair<IHardware, IList<ISensor>> pair in sensors) {
        if (hardwareNames.Value) {
          if (y > topMargin)
            y += hardwareLineHeight - sensorLineHeight;
          y += hardwareLineHeight;
        }
        y += pair.Value.Count * sensorLineHeight;
      }      
      if (sensors.Count == 0)
        y += 4 * sensorLineHeight + hardwareLineHeight;
      y += bottomMargin;
      this.Size = new Size(width, y);
    }

    private void DrawImageWidthBorder(Graphics g, int width, int height, 
      Image back, int t, int b, int l, int r) 
    {
      GraphicsUnit u = GraphicsUnit.Pixel;

      g.DrawImage(back, new Rectangle(0, 0, l, t),
            new Rectangle(0, 0, l, t), u);
      g.DrawImage(back, new Rectangle(l, 0, width - l - r, t),
        new Rectangle(l, 0, back.Width - l - r, t), u);
      g.DrawImage(back, new Rectangle(width - r, 0, r, t),
        new Rectangle(back.Width - r, 0, r, t), u);

      g.DrawImage(back, new Rectangle(0, t, l, height - t - b),
        new Rectangle(0, t, l, back.Height - t - b), u);
      g.DrawImage(back, new Rectangle(l, t, width - l - r, height - t - b),
        new Rectangle(l, t, back.Width - l - r, back.Height - t - b), u);
      g.DrawImage(back, new Rectangle(width - r, t, r, height - t - b),
        new Rectangle(back.Width - r, t, r, back.Height - t - b), u);

      g.DrawImage(back, new Rectangle(0, height - b, l, b),
        new Rectangle(0, back.Height - b, l, b), u);
      g.DrawImage(back, new Rectangle(l, height - b, width - l - r, b),
        new Rectangle(l, back.Height - b, back.Width - l - r, b), u);
      g.DrawImage(back, new Rectangle(width - r, height - b, r, b),
        new Rectangle(back.Width - r, back.Height - b, r, b), u);
    }

    private void DrawBackground(Graphics g) {
      int w = Size.Width;
      int h = Size.Height;      

      if (w != background.Width || h != background.Height || back_isChanged) {
        back_isChanged = false;
        background.Dispose();
        background = new Bitmap(w, h, PixelFormat.Format32bppPArgb);
        using (Graphics graphics = Graphics.FromImage(background)) {

          DrawImageWidthBorder(graphics, w, h, back, topBorder, bottomBorder, 
            leftBorder, rightBorder);    
          if (fore != null)
            DrawImageWidthBorder(graphics, w, h, fore, topBorder, bottomBorder,
            leftBorder, rightBorder);

          if (image != null) {
            int width = w - leftBorder - rightBorder;
            int height = h - topBorder - bottomBorder;
            float xRatio = width / (float)image.Width;
            float yRatio = height / (float)image.Height;
            float destWidth, destHeight;
            float xOffset, yOffset;
            if (xRatio < yRatio) {
              destWidth = width;
              destHeight = image.Height * xRatio;
              xOffset = 0;
              yOffset = 0.5f * (height - destHeight);
            } else {
              destWidth = image.Width * yRatio;
              destHeight = height;
              xOffset = 0.5f * (width - destWidth);
              yOffset = 0;
            }

            graphics.DrawImage(image,
              new RectangleF(leftBorder + xOffset, topBorder + yOffset, 
                destWidth, destHeight));
          }
        }
      }

      g.DrawImageUnscaled(background, 0, 0);
      if(BackgroundImageCoverOpacity!=0) g.FillRectangle(new SolidBrush(Color.FromArgb(BackgroundImageCoverOpacity, BackgroundImageCoverColor?Color.White:Color.Black)), new Rectangle(0, 0, w, h));
    }
        
    /*private void DrawProgress(Graphics g, float x, float y, 
      float width, float height, float progress) 
    {
      g.DrawImage(barBack, 
        new RectangleF(x + width * progress, y, width * (1 - progress), height), 
        new RectangleF(barBack.Width * progress, 0, 
          (1 - progress) * barBack.Width, barBack.Height), 
        GraphicsUnit.Pixel);
      g.DrawImage(barFore,
        new RectangleF(x, y, width * progress, height),
        new RectangleF(0, 0, progress * barFore.Width, barFore.Height),
        GraphicsUnit.Pixel);
    }*/

    protected override void OnPaint(PaintEventArgs e) {
      Graphics g = e.Graphics;
      int w = Size.Width;

      g.Clear(Color.Transparent);
      
      DrawBackground(g);

      int x;
      int y = topMargin;

      if (sensors.Count == 0) {
        x = leftBorder + 1;
        g.DrawString("在主窗口右键一个传感器，选择“在悬浮窗显示”。", 
          smallFont, Brushes.White,
          new Rectangle(x, y - 1, w - rightBorder - x, 0));
      }

      foreach (KeyValuePair<IHardware, IList<ISensor>> pair in sensors) {
        if (hardwareNames.Value) {
          if (y > topMargin)
            y += hardwareLineHeight - sensorLineHeight;
          x = leftBorder + 1;
          if(HardwareBackgroundOpacity!=0) g.FillRectangle(new SolidBrush(Color.FromArgb(HardwareBackgroundOpacity, 0, 0, 0)), new Rectangle(0, y - 8, w, hardwareLineHeight+5));
          g.DrawImage(HardwareTypeImage.Instance.GetImage(pair.Key.HardwareType),
            new Rectangle(x, y + 1, iconSize, iconSize));
          x += iconSize + 1;
          g.DrawString(Translate.toChinese(pair.Key.Name), largeFont, Brushes.White,
            new Rectangle(x, y - 1, w - rightBorder - x, 0), 
            stringFormat);
          y += hardwareLineHeight;
        }

        foreach (ISensor sensor in pair.Value) {
          int remainingWidth;


          if ((sensor.SensorType != SensorType.Load &&
               sensor.SensorType != SensorType.Control &&
               sensor.SensorType != SensorType.Level) || !sensor.Value.HasValue) 
          {
            string formatted;

            if (sensor.Value.HasValue) {
              string format = "";
              switch (sensor.SensorType) {
                case SensorType.Voltage:
                  format = "{0:F3} V";
                  break;
                case SensorType.Clock:
                  format = "{0:F0} MHz";
                  break;
                case SensorType.Temperature:
                  format = "{0:F1} °C";
                  break;
                case SensorType.Fan:
                  format = "{0:F0} RPM";
                  break;
                case SensorType.Flow:
                  format = "{0:F0} L/h";
                  break;
                case SensorType.Power:
                  format = "{0:F1} W";
                  break;
                case SensorType.Data:
                  format = "{0:F1} GB";
                  break;
                case SensorType.SmallData:
                  format = "{0:F0} MB";
                  break;
                case SensorType.Factor:
                  format = "{0:F3}";
                  break;
              }

              if (sensor.SensorType == SensorType.Temperature &&
                unitManager.TemperatureUnit == TemperatureUnit.Fahrenheit) {
                formatted = string.Format("{0:F1} °F",
                  UnitManager.CelsiusToFahrenheit(sensor.Value));
              } else if(sensor.SensorType == SensorType.InternetSpeed){
                string result = "-";
                switch (sensor.Name){ 
                  case "Connection Speed": {
                    switch (sensor.Value){ 
                      case 100000000: result = "100Mbps";break;
                      case 1000000000: result = "1Gbps";break;
                      default: {
                        if(sensor.Value < 1024) result = string.Format("{0:F0} bps", sensor.Value);
                        else if(sensor.Value < 1048576) result = string.Format("{0:F1} Kbps", sensor.Value / 1024);
                        else if(sensor.Value < 1073741824) result = string.Format("{0:F1} Mbps", sensor.Value / 1048576);
                        else result = string.Format("{0:F1} Gbps", sensor.Value / 1073741824);
                      } break;
                    }
                  }break;
                  default:{
                    if(sensor.Value < 1048576) result = string.Format("{0:F1} KB/s", sensor.Value / 1024);
                    else result = string.Format("{0:F1} MB/s", sensor.Value / 1048576);
                  } break;
                }
                formatted =  result;
              } else {
                formatted = string.Format(format, sensor.Value);
              }
            } else {
              formatted = "-";
            }
            Brush sensorColor = darkWhite;
            if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
            {
              int value30 = Clamp((int)((sensor.Value - 25) * 14.2857f) * 3, 0, 1000);
              sensorColor = new SolidBrush(Color.FromArgb(progressColor[value30], progressColor[value30 + 1], progressColor[value30 + 2]));
            }
            g.DrawString(formatted, smallFont, sensorColor,
              new RectangleF(-1, y - 1, w - rightMargin + 3, 0),
              alignRightStringFormat);

            remainingWidth = w - (int)Math.Floor(g.MeasureString(formatted,
              smallFont, w, StringFormat.GenericTypographic).Width) -
              rightMargin;
          } else {
            /*DrawProgress(g, w - progressWidth - rightMargin,
              y + 0.35f * sensorLineHeight, progressWidth,
              0.6f * sensorLineHeight, 0.01f * sensor.Value.Value);*/
            if(ProgressOpacity!=0){
              int value30 = (int)(sensor.Value * 10f) * 3;
              g.FillRectangle(new SolidBrush(Color.FromArgb(ProgressOpacity, progressColor[value30], progressColor[value30 + 1], progressColor[value30 + 2])), new Rectangle(0, y - 3, (int)(w * 0.01f * sensor.Value.Value), sensorLineHeight+1));
            }
            string formattedProgress = sensor.Value.Value.ToString("#0.0") + "%";
            g.DrawString(formattedProgress, smallFont, darkWhite, new RectangleF(-1, y - 1, w - rightMargin + 3, 0),alignRightStringFormat);
            remainingWidth = w - (int)Math.Floor(g.MeasureString(formattedProgress, smallFont, w, StringFormat.GenericTypographic).Width) - rightMargin;
          }
           
          remainingWidth -= leftMargin + 2;
          if (remainingWidth > 0) {
            g.DrawString(Translate.toChinese(sensor), smallFont, darkWhite,
              new RectangleF(leftMargin - 1, y - 1, remainingWidth, 0), 
              trimStringFormat);
          }

          y += sensorLineHeight;
        }
      }
    }

    private class HardwareComparer : IComparer<IHardware> {
      public int Compare(IHardware x, IHardware y) {
        if (x == null && y == null)
          return 0;
        if (x == null)
          return -1;
        if (y == null)
          return 1;

        if (x.HardwareType != y.HardwareType)
          return x.HardwareType.CompareTo(y.HardwareType);

        return x.Identifier.CompareTo(y.Identifier);
      }
    }
        private Image TransparentImage(Image srcImage, float opacity)
        {
            float[][] nArray ={ new float[] {1, 0, 0, 0, 0},
                        new float[] {0, 1, 0, 0, 0},
                        new float[] {0, 0, 1, 0, 0},
                        new float[] {0, 0, 0, opacity, 0},
                        new float[] {0, 0, 0, 0, 1}};
            ColorMatrix matrix = new ColorMatrix(nArray);
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            Bitmap resultImage = new Bitmap(srcImage.Width, srcImage.Height);
            Graphics g = Graphics.FromImage(resultImage);
            g.DrawImage(srcImage, new Rectangle(0, 0, srcImage.Width, srcImage.Height), 0, 0, srcImage.Width, srcImage.Height, GraphicsUnit.Pixel, attributes);

            return resultImage;
        }
        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min; else if (value > max) return max; else return value;
        }
    }
}

