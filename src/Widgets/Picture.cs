/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// TGUI - Texus' Graphical User Interface
// Copyright (C) 2012-2016 Bruno Van de Velde (vdv_b@tgui.eu)
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented;
//    you must not claim that you wrote the original software.
//    If you use this software in a product, an acknowledgment
//    in the product documentation would be appreciated but is not required.
//
// 2. Altered source versions must be plainly marked as such,
//    and must not be misrepresented as being the original software.
//
// 3. This notice may not be removed or altered from any source distribution.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Security;
using System.Runtime.InteropServices;
using SFML.System;
using SFML.Graphics;

namespace TGUI
{
	public class Picture : ClickableWidget
	{
		public Picture(string filename = "")
			: base(tguiPicture_create())
		{
			if (filename.Length > 0)
				Renderer.Texture = new Texture(filename);
		}

		public Picture(Texture texture = null)
			: base(tguiPicture_create())
		{
			if (texture != null)
				Renderer.Texture = texture;
		}

		protected internal Picture(IntPtr cPointer)
			: base(cPointer)
		{
		}

		public Picture(Picture copy)
			: base(copy)
		{
		}

		public new PictureRenderer Renderer
		{
			get { return new PictureRenderer(tguiWidget_getRenderer(CPointer)); }
		}

		public bool IgnoreMouseEvents
		{
			get { return tguiPicture_isIgnoringMouseEvents(CPointer); }
			set { tguiPicture_ignoreMouseEvents(CPointer, value); }
		}


		protected override void InitSignals()
		{
			base.InitSignals();

			IntPtr error;

		    DoubleClickedCallback = new CallbackActionVector2f(ProcessDoubleClickedSignal);
		    tguiPicture_connect_onDoubleClick(CPointer, DoubleClickedCallback, out error);
		    if (error != IntPtr.Zero)
				throw new TGUIException(Util.GetStringFromC_ASCII(error));
		}

		private void ProcessDoubleClickedSignal(Vector2f pos)
		{
			if (DoubleClicked != null)
				DoubleClicked(this, new SignalArgsVector2f(pos));
		}

		/// <summary>Event handler for the DoubleClicked signal</summary>
		public event EventHandler<SignalArgsVector2f> DoubleClicked = null;

	    private CallbackActionVector2f DoubleClickedCallback;

	    #region Imports

		[DllImport("ctgui-0.8.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected IntPtr tguiPicture_create();

		[DllImport("ctgui-0.8.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiPicture_ignoreMouseEvents(IntPtr cPointer, bool ignore);

		[DllImport("ctgui-0.8.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected bool tguiPicture_isIgnoringMouseEvents(IntPtr cPointer);

        [DllImport("ctgui-0.8.dll", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiPicture_connect_onDoubleClick(IntPtr cPointer, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackActionVector2f func, out IntPtr error);

		#endregion
	}
}
