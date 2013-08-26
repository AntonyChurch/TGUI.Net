/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// TGUI - Texus's Graphical User Interface
// Copyright (C) 2012-2013 Bruno Van de Velde (vdv_b@tgui.eu)
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
using SFML.Window;
using SFML.Graphics;
using Tao.OpenGl;

namespace TGUI
{
    public class ChildWindow : Container, WidgetBorders
    {
        /// Title alignments
        public enum Alignment
        {
            /// Places the title on the left side of the title bar
            Left,

            /// Places the title in the middle of the title bar
            Centered,

            /// Places the title on the right side of the title bar
            Right
        };

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public event EventHandler<CallbackArgs> ClosedCallback;
        public event EventHandler<CallbackArgs> MovedCallback;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private string     m_LoadedConfigFile = "";
   
        private Vector2f   m_Size = new Vector2f(0, 0);

        private Color      m_BackgroundColor = new Color(0, 0, 0);
        private Texture    m_BackgroundTexture = null;
        private Sprite     m_BackgroundSprite = new Sprite();

        private Impl.Sprite m_IconTexture = new Impl.Sprite();

        private Text       m_TitleText = new Text();
        private uint       m_TitleBarHeight = 0;
        private bool       m_SplitImage = false;
        private Vector2f   m_DraggingPosition = new Vector2f(0, 0);
        private uint       m_DistanceToSide = 0;
        private Alignment  m_TitleAlignment = Alignment.Centered;
        private Color      m_BorderColor = new Color(0, 0, 0);
        private Borders    m_Borders;

        private Impl.Sprite m_TextureTitleBar_L = new Impl.Sprite();
        private Impl.Sprite m_TextureTitleBar_M = new Impl.Sprite();
        private Impl.Sprite m_TextureTitleBar_R = new Impl.Sprite();

        private Button     m_CloseButton = new Button();

        private bool       m_KeepInParent = false;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Constructor, only intended for internal use
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal ChildWindow ()
        {
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Copy constructor
        ///
        /// \param copy  Instance to copy
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ChildWindow (ChildWindow copy) : base(copy)
        {
            ClosedCallback = copy.ClosedCallback;
            MovedCallback = copy.MovedCallback;

            m_LoadedConfigFile  = copy.m_LoadedConfigFile;
            m_Size              = copy.m_Size;
            m_BackgroundColor   = copy.m_BackgroundColor;
            m_BackgroundTexture = copy.m_BackgroundTexture;
            m_TitleText         = new Text(copy.m_TitleText);
            m_TitleBarHeight    = copy.m_TitleBarHeight;
            m_SplitImage        = copy.m_SplitImage;
            m_DraggingPosition  = copy.m_DraggingPosition;
            m_DistanceToSide    = copy.m_DistanceToSide;
            m_TitleAlignment    = copy.m_TitleAlignment;
            m_BorderColor       = copy.m_BorderColor;
            m_Borders           = copy.m_Borders;
            m_CloseButton       = new Button(copy.m_CloseButton);
            m_KeepInParent      = copy.m_KeepInParent;
            
            Global.TextureManager.CopyTexture(copy.m_IconTexture, m_IconTexture);
            Global.TextureManager.CopyTexture(copy.m_TextureTitleBar_L, m_TextureTitleBar_L);
            Global.TextureManager.CopyTexture(copy.m_TextureTitleBar_M, m_TextureTitleBar_M);
            Global.TextureManager.CopyTexture(copy.m_TextureTitleBar_R, m_TextureTitleBar_R);

            if (copy.m_BackgroundTexture != null)
            {
                m_BackgroundSprite.Texture = m_BackgroundTexture;
                m_BackgroundSprite.Scale = new Vector2f(m_Size.X / m_BackgroundTexture.Size.X, m_Size.Y / m_BackgroundTexture.Size.Y);
                m_BackgroundSprite.Color = new Color (255, 255, 255, m_Opacity);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Loads the widget.
        ///
        /// \param configFileFilename  Filename of the config file.
        ///
        /// The config file must contain a ChildWindow section with the needed information.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ChildWindow (string configFileFilename)
        {
            m_LoadedConfigFile = configFileFilename;

            // Parse the config file
            ConfigFile configFile = new ConfigFile (configFileFilename, "ChildWindow");

            // Find the folder that contains the config file
            string configFileFolder = configFileFilename.Substring(0, configFileFilename.LastIndexOfAny(new char[] {'/', '\\'}) + 1);

            // Loop over all properties
            for (int i = 0; i < configFile.Properties.Count; ++i)
            {
                if (configFile.Properties[i] == "backgroundcolor")
                    m_BackgroundColor = configFile.ReadColor(i);
                else if (configFile.Properties[i] == "titlecolor")
                    m_TitleText.Color = configFile.ReadColor(i);
                else if (configFile.Properties[i] == "bordercolor")
                    m_BorderColor = configFile.ReadColor(i);
                else if (configFile.Properties[i] == "titlebarimage")
                {
                    configFile.ReadTexture (i, configFileFolder, m_TextureTitleBar_M);
                    m_SplitImage = false;
                }
                else if (configFile.Properties[i] == "closebuttonseparatehoverimage")
                    m_CloseButton.m_SeparateHoverImage = configFile.ReadBool(i);
                else if (configFile.Properties[i] == "closebuttonnormalimage")
                    configFile.ReadTexture(i, configFileFolder, m_CloseButton.m_TextureNormal_M);
                else if (configFile.Properties[i] == "closebuttonhoverimage")
                    configFile.ReadTexture(i, configFileFolder, m_CloseButton.m_TextureHover_M);
                else if (configFile.Properties[i] == "closebuttondownimage")
                    configFile.ReadTexture (i, configFileFolder, m_CloseButton.m_TextureDown_M);
                else if (configFile.Properties[i] == "borders")
                {
                    Borders borders;
                    if (Internal.ExtractBorders (configFile.Values [i], out borders))
                        Borders = borders;
                }
                else if (configFile.Properties[i] == "distancetoside")
                    DistanceToSide = Convert.ToUInt32(configFile.Values [i]);
                else
                    Internal.Output("TGUI warning: Unrecognized property '" + configFile.Properties[i]
                                    + "' in section ChildWindow in " + configFileFilename + ".");
            }

            // Initialize the close button if it was loaded
            if (m_CloseButton.m_TextureNormal_M.texture != null)
            {
                // Check if optional textures were loaded
                if (m_CloseButton.m_TextureHover_M.texture != null)
                {
                    m_CloseButton.m_WidgetPhase |= (byte)WidgetPhase.Hover;
                }
                if (m_CloseButton.m_TextureDown_M.texture != null)
                {
                    m_CloseButton.m_WidgetPhase |= (byte)WidgetPhase.MouseDown;
                }

                m_CloseButton.m_Size = new Vector2f(m_CloseButton.m_TextureNormal_M.Size.X,
                                                    m_CloseButton.m_TextureNormal_M.Size.Y);
            }
            else // Close button wan't loaded
            {
                throw new Exception("Missing a CloseButtonNormalImage property in section ChildWindow in "
                                    + configFileFilename + ".");
            }

            // Make sure the required texture was loaded
            if ((m_TextureTitleBar_M.texture != null))
            {
                m_TitleBarHeight = (uint)m_TextureTitleBar_M.Size.Y;
            }
            else
            {
                throw new Exception("Not all needed images were loaded for the child window. Is the ChildWindow section in "
                                    + configFileFilename + " complete?");
            }

            // Set the size of the title text
            m_TitleText.CharacterSize = (uint)(m_TitleBarHeight * 8.0 / 10.0);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Destructor
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ~ChildWindow ()
        {
            if (m_TextureTitleBar_L.texture != null)   Global.TextureManager.RemoveTexture(m_TextureTitleBar_L);
            if (m_TextureTitleBar_M.texture != null)   Global.TextureManager.RemoveTexture(m_TextureTitleBar_M);
            if (m_TextureTitleBar_R.texture != null)   Global.TextureManager.RemoveTexture(m_TextureTitleBar_R);

            if (m_IconTexture.texture != null)
                Global.TextureManager.RemoveTexture(m_IconTexture);

            if (m_IconTexture.texture != null)
                Global.TextureManager.RemoveTexture(m_IconTexture);

            m_CloseButton = null;
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Returns the filename of the config file that was used to load the widget.
        ///
        /// \return Filename of loaded config file.
        ///         Empty string when no config file was loaded yet.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string LoadedConfigFile
        {
            get
            {
                return m_LoadedConfigFile;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes the size of the widget.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Vector2f Size
        {
            get
            {
                return m_Size;
            }
            set
            {
                m_Size = value;
            
                // If there is a background texture then resize it
                if (m_BackgroundTexture != null)
                    m_BackgroundSprite.Scale = new Vector2f(m_Size.X / m_BackgroundTexture.Size.X,
                                                            m_Size.Y / m_BackgroundTexture.Size.Y);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes the background texture of the child window.
        ///
        /// \param texture  Texture that should be used as background of the child window
        ///
        /// If the texture has a different size than the child window then it will be scaled to fill the whole window.
        /// Pass null to this function to remove the background texture.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Texture BackgroundTexture
        {
            get
            {
                return m_BackgroundTexture;
            }
            set
            {
                m_BackgroundTexture = value;

                if (value != null)
                {
                    m_BackgroundSprite.Texture = m_BackgroundTexture;
                    m_BackgroundSprite.Scale = new Vector2f(m_Size.X / m_BackgroundTexture.Size.X,
                                                            m_Size.Y / m_BackgroundTexture.Size.Y);
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the height of the title bar.
        ///
        /// The default height is the height of the title bar image that is loaded in the constructor.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint TitleBarHeight
        {
            get
            {
                return m_TitleBarHeight;
            }
            set
            {
                m_TitleBarHeight = value;

                // Set the size of the close button
                m_CloseButton.Size = new Vector2f((float)(m_TitleBarHeight) / m_TextureTitleBar_M.Size.Y * m_CloseButton.Size.X,
                                                  (float)(m_TitleBarHeight) / m_TextureTitleBar_M.Size.Y * m_CloseButton.Size.Y);

                // Set the size of the text in the title bar
                m_TitleText.CharacterSize = (uint)(m_TitleBarHeight * 8.0 / 10.0);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the background color of the child window.
        ///
        /// The background is fully transparent by default.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Color BackgroundColor
        {
            get
            {
                return m_BackgroundColor;
            }
            set
            {
                m_BackgroundColor = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes the transparency of the widget.
        ///
        /// 0 is completely transparent, while 255 (default) means fully opaque.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override byte Transparency
        {
            set
            {
                base.Transparency = value;

                m_BackgroundSprite.Color = new Color(255, 255, 255, m_Opacity);

                m_IconTexture.sprite.Color = new Color(255, 255, 255, m_Opacity);

                m_TextureTitleBar_L.sprite.Color = new Color(255, 255, 255, m_Opacity);
                m_TextureTitleBar_M.sprite.Color = new Color(255, 255, 255, m_Opacity);
                m_TextureTitleBar_R.sprite.Color = new Color(255, 255, 255, m_Opacity);

                m_CloseButton.Transparency = m_Opacity;

                m_TitleText.Color = new Color(m_TitleText.Color.R, m_TitleText.Color.G, m_TitleText.Color.B, m_Opacity);

                m_BackgroundColor.A = m_Opacity;
                m_BorderColor.A = m_Opacity;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the title that is displayed in the title bar of the child window.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Title
        {
            get
            {
                return m_TitleText.DisplayedString;
            }
            set
            {
                m_TitleText.DisplayedString = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the color of the title that is displayed in the title bar of the child window.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Color TitleColor
        {
            get
            {
                return m_TitleText.Color;
            }
            set
            {
                m_TitleText.Color = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the color of the borders.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Color BorderColor
        {
            get
            {
                return m_BorderColor;
            }
            set
            {
                m_BorderColor = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the border width and border height of the child window.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Borders Borders
        {
            get
            {
                return m_Borders;
            }
            set
            {
                m_Borders = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the distance between the title and the side of the title bar.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint DistanceToSide
        {
            get
            {
                return m_DistanceToSide;
            }
            set
            {
                m_DistanceToSide = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the title alignment.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Alignment TitleAlignment
        {
            get
            {
                return m_TitleAlignment;
            }
            set
            {
                m_TitleAlignment = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes the icon in the top left corner of the child window.
        ///
        /// \param filename  Filename of the icon image
        ///
        /// There is no icon by default.
        ///
        /// Passing an empty filename will remove the current icon.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetIcon (string filename)
        {
            // If a texture has already been loaded then remove it first
            if (m_IconTexture.texture != null)
                Global.TextureManager.RemoveTexture(m_IconTexture);

            if (filename != "")
            {
                // Load the icon image
                Global.TextureManager.GetTexture (filename, m_IconTexture);
                m_IconTexture.sprite.Scale = new Vector2f((float)(m_TitleBarHeight) / m_TextureTitleBar_M.Size.Y,
                                                          (float)(m_TitleBarHeight) / m_TextureTitleBar_M.Size.Y);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Destroys the window.
        ///
        /// When no callback is requested when closing the window, this function will be called automatically.
        ///
        /// When you requested a callback then you get the opportunity to cancel the closure of the window.
        /// If you want to keep it open then don't do anything, if you want to close it then just call this function.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Destroy ()
        {
            m_Parent.Remove (this);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Set the child window to be kept inside its parent.
        ///
        /// When it's set to true, the child window will always be kept automatically inside its parent.
        /// It will be fully kept on left, right and top. At the bottom of the parent only the title bar will be kept inside.
        /// It's set to false by default.
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool KeepInParent
        {
            get
            {
                return m_KeepInParent;
            }
            set
            {
                m_KeepInParent = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \brief Changes/Returns the position of the widget
        ///
        /// The default position of a transformable widget is (0, 0).
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Vector2f Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                if (m_KeepInParent)
                {
                    if (value.Y < 0)
                        base.Position = new Vector2f(Position.X, 0);
                    else if (value.Y > m_Parent.Size.Y - m_TitleBarHeight)
                        base.Position = new Vector2f(Position.X, m_Parent.Size.Y - m_TitleBarHeight);
                    else
                        base.Position = new Vector2f(Position.X, value.Y);

                    if (value.X < 0)
                        base.Position = new Vector2f(0, Position.Y);
                    else if (value.X > m_Parent.Size.X - Size.X)
                        base.Position = new Vector2f(m_Parent.Size.X - Size.X, Position.Y);
                    else
                        base.Position = new Vector2f(value.X, Position.Y);
                }
                else
                    base.Position = value;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal override bool MouseOnWidget(float x, float y)
        {
            // Check if the mouse is on top of the title bar
            if (Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_TitleBarHeight + m_Borders.Top)).Contains(x, y))
            {
                m_EventManager.MouseNotOnWidget();
                return true;
            }
            else
            {
                // Check if the mouse is inside the child window
                if (Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_Size.Y + m_Borders.Top + m_Borders.Bottom)).Contains(x, y - m_TitleBarHeight))
                    return true;
                else
                {
                    if (m_MouseHover)
                        MouseLeftWidget();

                    // Tell the widgets inside the child window that the mouse is no longer on top of them
                    m_CloseButton.MouseNotOnWidget(); 
                    m_EventManager.MouseNotOnWidget();
                    m_MouseHover = false;
                    return false;
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal override void OnMouseMoved (MouseMoveEventArgs e)
        {
            m_MouseHover = true; 

            // Check if you are dragging the child window
            if (m_MouseDown == true)
            {
                // Move the child window
                Position = new Vector2f(Position.X + (e.X - Position.X - m_DraggingPosition.X), Position.Y + (e.Y - Position.Y - m_DraggingPosition.Y));

                // Add the callback (if the user requested it)
                if (MovedCallback != null)
                {
                    m_Callback.Trigger = CallbackTrigger.Moved;
                    m_Callback.Position = Position;
                    MovedCallback (this, m_Callback);
                }
            }

            // Check if the mouse is on top of the title bar
            if (Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_TitleBarHeight)).Contains(e.X, e.Y))
            {
                // Temporary set the close button to the correct position
                m_CloseButton.Position = new Vector2f(Position.X + ((m_Size.X + m_Borders.Left + m_Borders.Right - m_DistanceToSide - m_CloseButton.Size.X)),
                                                      Position.Y + ((m_TitleBarHeight / 2.0f) - (m_CloseButton.Size.X / 2.0f)));

                // Send the hover event to the close button
                if (m_CloseButton.MouseOnWidget(e.X, e.Y))
                    m_CloseButton.OnMouseMoved(e);

                // Reset the position of the button
                m_CloseButton.Position = new Vector2f(0, 0);
                return;
            }
            else // The mouse is not on top of the title bar
            {
                // Don't send the event to the widgets when the mouse is on top of the borders
                if ((Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_Size.Y + m_Borders.Top + m_Borders.Bottom + m_TitleBarHeight)).Contains(e.X, e.Y))
                    && (Transform.TransformRect(new FloatRect(m_Borders.Left, m_TitleBarHeight + m_Borders.Top, m_Size.X, m_Size.Y)).Contains(e.X, e.Y) == false))
                {
                    return;
                }
            }

            e.X -= (int)m_Borders.Left;
            e.Y -= (int)(m_TitleBarHeight + m_Borders.Top);
            base.OnMouseMoved(e);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal override void OnLeftMousePressed (MouseButtonEventArgs e)
        {
            // Move the childwindow to the front when clicking on it
            m_Parent.MoveWidgetToFront(this);

            // Check if the mouse is on top of the title bar
            if (Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_TitleBarHeight)).Contains(e.X, e.Y))
            {
                // Temporary set the close button to the correct position
                m_CloseButton.Position = new Vector2f(Position.X + ((m_Size.X + m_Borders.Left + m_Borders.Right - m_DistanceToSide - m_CloseButton.Size.X)),
                                                      Position.Y + ((m_TitleBarHeight / 2.0f) - (m_CloseButton.Size.X / 2.0f)));

                // Send the mouse press event to the close button
                if (m_CloseButton.MouseOnWidget(e.X, e.Y))
                    m_CloseButton.OnLeftMousePressed(e);
                else
                {
                    // The mouse went down on the title bar
                    m_MouseDown = true;

                    // Remember where we are dragging the title bar
                    m_DraggingPosition.X = e.X - Position.X;
                    m_DraggingPosition.Y = e.Y - Position.Y;
                }

                // Reset the position of the button
                m_CloseButton.Position = new Vector2f(0, 0);
                return;
            }
            else // The mouse is not on top of the title bar
            {
                // When the mouse is not on the title bar, the mouse can't be on the close button
                if (m_CloseButton.m_MouseHover)
                    m_CloseButton.MouseNotOnWidget();

                // Don't send the event to the widgets when the mouse is on top of the borders
                if ((Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_Size.Y + m_Borders.Top + m_Borders.Bottom + m_TitleBarHeight)).Contains(e.X, e.Y))
                    && (Transform.TransformRect(new FloatRect(m_Borders.Left, m_TitleBarHeight + m_Borders.Top, m_Size.X, m_Size.Y)).Contains(e.X, e.Y) == false))
                {
                    return;
                }
            }

            e.X -= (int)m_Borders.Left;
            e.Y -= (int)(m_TitleBarHeight + m_Borders.Top);
            base.OnLeftMousePressed(e);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal override void OnLeftMouseReleased (MouseButtonEventArgs e)
        {
            // Check if the mouse is on top of the title bar
            if (Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_TitleBarHeight)).Contains(e.X, e.Y))
            {
                // Temporary set the close button to the correct position
                m_CloseButton.Position = new Vector2f(Position.X + ((m_Size.X + m_Borders.Left + m_Borders.Right - m_DistanceToSide - m_CloseButton.Size.X)),
                                                      Position.Y + ((m_TitleBarHeight / 2.0f) - (m_CloseButton.Size.X / 2.0f)));

                m_MouseDown = false;

                // Check if the close button was clicked
                if (m_CloseButton.m_MouseDown == true)
                {
                    m_CloseButton.m_MouseDown = false;

                    // Check if the mouse is still on the close button
                    if (m_CloseButton.MouseOnWidget(e.X, e.Y))
                    {
                        // If a callback was requested then send it
                        if (ClosedCallback != null)
                        {
                            m_Callback.Trigger = CallbackTrigger.Closed;
                            ClosedCallback (this, m_Callback);
                        }
                        else // The user won't stop the closing, so destroy the window
                        {
                            Destroy();
                            return;
                        }
                    }
                }

                // Reset the position of the button
                m_CloseButton.Position = new Vector2f(0, 0);

                // Tell the widgets that the mouse is no longer down
                m_EventManager.MouseNoLongerDown();
                return;
            }
            else // The mouse is not on top of the title bar
            {
                m_MouseDown = false;
                m_CloseButton.MouseNoLongerDown();

                // Don't send the event to the widgets when the mouse is on top of the borders
                if ((Transform.TransformRect(new FloatRect(0, 0, m_Size.X + m_Borders.Left + m_Borders.Right, m_Size.Y + m_Borders.Top + m_Borders.Bottom + m_TitleBarHeight)).Contains(e.X, e.Y))
                    && (Transform.TransformRect(new FloatRect(m_Borders.Left, m_TitleBarHeight + m_Borders.Top, m_Size.X, m_Size.Y)).Contains(e.X, e.Y) == false))
                {
                    // Tell the widgets that the mouse has been released
                    m_EventManager.MouseNoLongerDown();
                    return;
                }
            }

            e.X -= (int)m_Borders.Left;
            e.Y -= (int)(m_TitleBarHeight + m_Borders.Top);
            base.OnLeftMouseReleased(e);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal override void OnMouseWheelMoved (MouseWheelEventArgs e)
        {
            e.X -= (int)m_Borders.Left;
            e.Y -= (int)(m_TitleBarHeight + m_Borders.Top);
            base.OnMouseWheelMoved(e);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal override void MouseNoLongerDown()
        {
            base.MouseNoLongerDown ();
            m_CloseButton.MouseNoLongerDown ();
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        // This function is called when the widget is added to a container.
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected internal override void Initialize(Container parent)
        {
            base.Initialize(parent);
            GlobalFont = parent.GlobalFont;
            m_TitleText.Font = GlobalFont;
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \internal
        // Draws the widget on the render target.
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Draw(RenderTarget target, RenderStates states)
        {
            // Calculate the scale factor of the view
            float scaleViewX = target.Size.X / target.GetView().Size.X;
            float scaleViewY = target.Size.Y / target.GetView().Size.Y;

            Vector2f viewPosition = (target.GetView().Size / 2.0f) - target.GetView().Center;

            // Get the global position
            Vector2f topLeftPanelPosition = states.Transform.TransformPoint(m_Position.X + m_Borders.Left + viewPosition.X,
                                                                            m_Position.Y + m_TitleBarHeight + m_Borders.Top + viewPosition.Y);
            Vector2f bottomRightPanelPosition = states.Transform.TransformPoint(m_Position.X + m_Size.X + m_Borders.Left + viewPosition.X,
                                                                                m_Position.Y + m_TitleBarHeight + m_Size.Y + m_Borders.Top + viewPosition.Y);
            Vector2f topLeftTitleBarPosition;
            Vector2f bottomRightTitleBarPosition;

            if (m_IconTexture.texture != null)
                topLeftTitleBarPosition = states.Transform.TransformPoint(m_Position.X + 2*m_DistanceToSide + (m_IconTexture.Size.X * m_IconTexture.sprite.Scale.X) + viewPosition.X,
                                                                          m_Position.Y + viewPosition.Y);
            else
                topLeftTitleBarPosition = states.Transform.TransformPoint(m_Position.X + m_DistanceToSide + viewPosition.X, m_Position.Y + viewPosition.Y);

            bottomRightTitleBarPosition = states.Transform.TransformPoint(m_Position.X + m_Size.X + m_Borders.Left + m_Borders.Right - (2*m_DistanceToSide) - m_CloseButton.Size.X + viewPosition.X,
                                                                          m_Position.Y + m_TitleBarHeight + viewPosition.Y);

            // Adjust the transformation
            states.Transform *= Transform;

            Transform oldTransform = states.Transform;

            // Check if the title bar image is split
            if (m_SplitImage)
            {
                // Split image is not supported yet
                return;
            }
            else // The title bar image isn't split
            {
                // Scale the title bar
                states.Transform.Scale((float)(m_Size.X + m_Borders.Left + m_Borders.Right) / m_TextureTitleBar_M.Size.X,
                                       (float)(m_TitleBarHeight) / m_TextureTitleBar_M.Size.Y);

                // Draw the title bar
                target.Draw(m_TextureTitleBar_M.sprite, states);

                // Undo the scaling
                states.Transform.Scale((float)m_TextureTitleBar_M.Size.X / (m_Size.X + m_Borders.Left + m_Borders.Right),
                                       (float)m_TextureTitleBar_M.Size.Y / m_TitleBarHeight);
            }

            // Draw a window icon if one was set
            if (m_IconTexture.texture != null)
            {
                states.Transform.Translate(m_DistanceToSide, (m_TitleBarHeight - (m_IconTexture.Size.Y * m_IconTexture.sprite.Scale.Y)) / 2.0f);
                target.Draw(m_IconTexture.sprite, states);
                states.Transform.Translate(m_IconTexture.Size.X * m_IconTexture.sprite.Scale.X,
                                           (m_TitleBarHeight - (m_IconTexture.Size.Y * m_IconTexture.sprite.Scale.Y)) / -2.0f);
            }

            // Get the old clipping area
            int[] scissor = new int[4];
            Gl.glGetIntegerv(Gl.GL_SCISSOR_BOX, scissor);

            int scissorLeft, scissorTop, scissorRight, scissorBottom;

            // Check if there is a title
            if (m_TitleText.DisplayedString.Length > 0)
            {
                // Calculate the clipping area
                scissorLeft = System.Math.Max((int)(topLeftTitleBarPosition.X * scaleViewX), scissor[0]);
                scissorTop = System.Math.Max((int)(topLeftTitleBarPosition.Y * scaleViewY), (int)(target.Size.Y) - scissor[1] - scissor[3]);
                scissorRight = System.Math.Min((int)(bottomRightTitleBarPosition.X * scaleViewX), scissor[0] + scissor[2]);
                scissorBottom = System.Math.Min((int)(bottomRightTitleBarPosition.Y * scaleViewY), (int)(target.Size.Y) - scissor[1]);

                // If the widget outside the window then don't draw anything
                if (scissorRight < scissorLeft)
                    scissorRight = scissorLeft;
                else if (scissorBottom < scissorTop)
                    scissorTop = scissorBottom;

                // Set the clipping area
                Gl.glScissor(scissorLeft, (int)target.Size.Y - scissorBottom, scissorRight - scissorLeft, scissorBottom - scissorTop);

                // Draw the text, depending on the alignment
                if (m_TitleAlignment == Alignment.Left)
                {
                    states.Transform.Translate(m_DistanceToSide, 0);
                    target.Draw(m_TitleText, states);
                }
                else if (m_TitleAlignment == Alignment.Centered)
                {
                    if (m_IconTexture.texture != null)
                        states.Transform.Translate(m_DistanceToSide + (((m_Size.X + m_Borders.Left + m_Borders.Right) - 4*m_DistanceToSide - (m_IconTexture.Size.X * m_IconTexture.sprite.Scale.X) - m_CloseButton.Size.X - m_TitleText.GetGlobalBounds().Width) / 2.0f), 0);
                    else
                        states.Transform.Translate(m_DistanceToSide + (((m_Size.X + m_Borders.Left + m_Borders.Right) - 3*m_DistanceToSide - m_CloseButton.Size.X - m_TitleText.GetGlobalBounds().Width) / 2.0f), 0);

                    target.Draw(m_TitleText, states);
                }
                else // if (m_TitleAlignment == TitleAlignmentRight)
                {
                    if (m_IconTexture.texture != null)
                        states.Transform.Translate((m_Size.X + m_Borders.Left + m_Borders.Right) - (m_IconTexture.Size.X * m_IconTexture.sprite.Scale.X) - 3*m_DistanceToSide - m_CloseButton.Size.X - m_TitleText.GetGlobalBounds().Width, 0);
                    else
                        states.Transform.Translate((m_Size.X + m_Borders.Left + m_Borders.Right) - 2*m_DistanceToSide - m_CloseButton.Size.X - m_TitleText.GetGlobalBounds().Width, 0);

                    target.Draw(m_TitleText, states);
                }

                // Reset the old clipping area
                Gl.glScissor(scissor[0], scissor[1], scissor[2], scissor[3]);
            }

            // Move the close button to the correct position
            states.Transform = oldTransform;
            states.Transform.Translate((m_Size.X + m_Borders.Left + m_Borders.Right) - m_DistanceToSide - m_CloseButton.Size.X, (m_TitleBarHeight - m_CloseButton.Size.Y) / 2.0f);

            // Draw the close button
            target.Draw(m_CloseButton, states);

            // Set the correct transformation
            states.Transform = oldTransform;
            states.Transform.Translate(0, m_TitleBarHeight);

            // Draw left border
            RectangleShape border = new RectangleShape(new Vector2f(m_Borders.Left, m_Size.Y + m_Borders.Top + m_Borders.Bottom));
            border.FillColor = m_BorderColor;
            target.Draw(border, states);

            // Draw top border
            border.Size = new Vector2f(m_Size.X + m_Borders.Left + m_Borders.Right, m_Borders.Top);
            target.Draw(border, states);

            // Draw right border
            border.Position = new Vector2f(m_Size.X + m_Borders.Left, 0);
            border.Size = new Vector2f(m_Borders.Right, m_Size.Y + m_Borders.Top + m_Borders.Bottom);
            target.Draw(border, states);

            // Draw bottom border
            border.Position = new Vector2f(0, m_Size.Y + m_Borders.Top);
            border.Size = new Vector2f(m_Size.X + m_Borders.Left + m_Borders.Right, m_Borders.Bottom);
            target.Draw(border, states);

            // Make room for the borders
            states.Transform.Translate(m_Borders.Left, m_Borders.Top);

            // Draw the background
            if (m_BackgroundColor.A != 0)
            {
                RectangleShape background = new RectangleShape(new Vector2f(m_Size.X, m_Size.Y));
                background.FillColor = m_BackgroundColor;
                target.Draw(background, states);
            }

            // Draw the background image if there is one
            if (m_BackgroundTexture != null)
                target.Draw(m_BackgroundSprite, states);

            // Calculate the clipping area
            scissorLeft = System.Math.Max((int)(topLeftPanelPosition.X * scaleViewX), scissor[0]);
            scissorTop = System.Math.Max((int)(topLeftPanelPosition.Y * scaleViewY), (int)(target.Size.Y) - scissor[1] - scissor[3]);
            scissorRight = System.Math.Min((int)(bottomRightPanelPosition.X * scaleViewX), scissor[0] + scissor[2]);
            scissorBottom = System.Math.Min((int)(bottomRightPanelPosition.Y * scaleViewY), (int)(target.Size.Y) - scissor[1]);

            // If the widget outside the window then don't draw anything
            if (scissorRight < scissorLeft)
                scissorRight = scissorLeft;
            else if (scissorBottom < scissorTop)
                scissorTop = scissorBottom;

            // Set the clipping area
            Gl.glScissor(scissorLeft, (int)target.Size.Y - scissorBottom, scissorRight - scissorLeft, scissorBottom - scissorTop);

            // Draw the widgets in the child window
            DrawContainer(target, states);

            // Reset the old clipping area
            Gl.glScissor(scissor[0], scissor[1], scissor[2], scissor[3]);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}

