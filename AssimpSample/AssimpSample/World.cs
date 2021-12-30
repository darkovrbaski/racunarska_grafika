// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_sceneComputer;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_sceneCD;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 2500.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private float m_diskRotationX = 0;

        private DispatcherTimer timer1;

        private DispatcherTimer timer2;

        private DispatcherTimer timer3;

        private DispatcherTimer timer4;

        private float m_cdTranslationZ = 0;

        private float m_diskTranslationZ = 480;

        private float m_diskRotationSpeed = 60;

        private double m_diskTranslationY = 115f;

        public bool LockControls { get; set; }

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene SceneComputer
        {
            get { return m_sceneComputer; }
            set { m_sceneComputer = value; }
        }

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene SceneCD
        {
            get { return m_sceneCD; }
            set { m_sceneCD = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            sceneFileName = "Computer.obj";
            this.m_sceneComputer = new AssimpScene(scenePath, sceneFileName, gl);
            sceneFileName = "CD.obj";
            this.m_sceneCD = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.ShadeModel(OpenGL.GL_FLAT);
            m_sceneComputer.LoadScene();
            m_sceneComputer.Initialize();
            m_sceneCD.LoadScene();
            m_sceneCD.Initialize();
            InitializeAnimation();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)m_width / m_height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            gl.Translate(0.0f, 300.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            DrawComputer(gl);
            DrawDisk(gl);
            DrawDesk(gl);
            DrawSurface(gl);
            DrawText(gl);
            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawComputer(OpenGL gl)
        {
            gl.PushMatrix();
            m_sceneComputer.Draw();
            DrawCd(gl);
            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawCd(OpenGL gl)
        {
            gl.PushMatrix();
            // cd izvucen 0.0f, 0.0f, 210f
            gl.Translate(0.0f, 0.0f, m_cdTranslationZ);
            m_sceneCD.Draw();
            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawDesk(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(0f, -320, 300f);
            gl.Scale(850f, 30f, 500f);
            gl.Color(0.6f, 0.3f, 0.2f);
            Cube cube = new Cube();
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(800f, -600, 300f);
            gl.Scale(30f, 278f, 450f);
            gl.Color(0.8f, 0.5f, 0.3);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-800f, -598, 300f);
            gl.Scale(30f, 278f, 450f);
            gl.Color(0.8f, 0.5f, 0.3);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawDisk(OpenGL gl)
        {
            gl.PushMatrix();
            // disk u citacu -480f, 115f, 480f
            gl.Translate(-480f, m_diskTranslationY, m_diskTranslationZ);
            gl.Rotate(0f, m_diskRotationX, 0f);
            gl.Scale(50f, 50f, 50f);
            gl.Rotate(90f, 0f, 0f);
            gl.Color(1f, 1f, 1f);
            Disk disk = new Disk();
            disk.Loops = 120;
            disk.Slices = 50;
            disk.InnerRadius = 0.3f;
            disk.OuterRadius = 1.5f;
            disk.CreateInContext(gl);
            disk.Render(gl, RenderMode.Render);
            gl.Rotate(180f, 0f, 0f);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            m_diskRotationX += m_diskRotationSpeed;
            gl.Flush();
        }

        private void DrawSurface(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Rotate(90f, 0f, 0f);
            gl.Translate(0f, 0f, 880f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(0.4f,0f,0f);
            gl.Vertex(2000f, -2000f);
            gl.Vertex(-2000f, -2000f);
            gl.Vertex(-2000f, 2000f);
            gl.Vertex(2000f, 2000f);
            gl.End();
            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawText(OpenGL gl)
        {
            gl.Viewport(m_width - 180, 0, m_width, m_height);
            gl.PushMatrix();
            gl.DrawText3D("tahoma italic", 10, 10, 10, "");
            gl.DrawText(0, 100, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "Predmet: Racunarska grafika");
            gl.DrawText(0, 98, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "________ __________ _______");
            gl.DrawText(0, 80, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "Sk.god: 2021/22.");
            gl.DrawText(0, 78, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "_______ ________");
            gl.DrawText(0, 60, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "Ime: Darko");
            gl.DrawText(0, 58, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "____ _____");
            gl.DrawText(0, 40, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "Prezime: Vrbaski");
            gl.DrawText(0, 38, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "________ _______");
            gl.DrawText(0, 20, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "Sifra zad: 6.2");
            gl.DrawText(0, 18, 1.0f, 1.0f, 0.0f, "tahoma italic", 10, "__________ ___");
            gl.PopMatrix();
            gl.Viewport(0, 0, m_width, m_height);
            gl.Flush();
        }

        private void InitializeAnimation()
        {
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(50);
            timer1.Tick += CdOpenAnimation;
            
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(120);
            timer2.Tick += DiskStopRotationAnimation;

            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(20);
            timer3.Tick += PopDiskAnimation;

            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromMilliseconds(35);
            timer4.Tick += CdCloseAnimation;
        }
        
        private void CdCloseAnimation(object sender, EventArgs e)
        {
            m_cdTranslationZ -= 10;
            if (m_cdTranslationZ == 0)
            {
                timer4.Stop();
                LockControls = false;
            }
        }

        private void PopDiskAnimation(object sender, EventArgs e)
        {
            m_diskTranslationY += 5;
            if (m_diskTranslationY >= 145)
            {
                timer3.Stop();
                timer4.Start();
            }
        }

        private void DiskStopRotationAnimation(object sender, EventArgs e)
        {
            m_diskRotationSpeed -= 1f;
            if (m_diskRotationSpeed == 0f)
            {
                timer2.Stop();
                timer3.Start();
            }
        }

        private void CdOpenAnimation(object sender, EventArgs e)
        {
            m_cdTranslationZ += 10f;
            m_diskTranslationZ += 10f;
            if (m_cdTranslationZ >= 210f)
            {
                timer1.Stop();
            }
        }

        public void StartAnimation()
        {
            LockControls = true;
            m_sceneDistance = 1700f;
            m_xRotation = 30;
            m_yRotation = 30;
            m_cdTranslationZ = 0;
            m_diskTranslationZ = 480f;
            m_diskRotationSpeed = 60f;
            m_diskTranslationY = 115f;

            timer3.Stop();
            timer4.Stop();

            timer1.Start();
            timer2.Start();
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_sceneComputer.Dispose();
                m_sceneCD.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
