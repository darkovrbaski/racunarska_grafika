﻿using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Computer"), "Computer.obj", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4:
                    Close();
                    break;
                case Key.W:
                    if (!m_world.LockControls && m_world.RotationX - 5.0f >= 0f) m_world.RotationX -= 5.0f;
                    break;
                case Key.S:
                    if (!m_world.LockControls && m_world.RotationX + 5.0f <= 90.0f) m_world.RotationX += 5.0f;
                    break;
                case Key.A:
                    if (!m_world.LockControls) m_world.RotationY -= 5.0f;
                    break;
                case Key.D:
                    if (!m_world.LockControls) m_world.RotationY += 5.0f;
                    break;
                case Key.Up:
                    if (!m_world.LockControls && m_world.ComputerZ - 20f > -220f) m_world.ComputerZ -= 20f;
                    break;
                case Key.Down:
                    if (!m_world.LockControls && m_world.ComputerZ + 20f < 240f) m_world.ComputerZ += 20f;
                    break;
                case Key.Left:
                    if (!m_world.LockControls && m_world.ComputerX - 20f > -260f) m_world.ComputerX -= 20f;
                    break;
                case Key.Right:
                    if (!m_world.LockControls && m_world.ComputerX + 20f < 260f) m_world.ComputerX += 20f;
                    break;
                case Key.Add:
                    if (!m_world.LockControls) m_world.SceneDistance -= 700.0f;
                    break;
                case Key.Subtract:
                    if (!m_world.LockControls) m_world.SceneDistance += 700.0f;
                    break;
                case Key.C:
                    if (!m_world.LockControls) m_world.StartAnimation();
                    break;
            }
        }
    }
}
