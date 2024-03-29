﻿// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Threading;
using SharpGL;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;

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

        /// <summary>
        ///	 Identifikatori tekstura za jednostavniji pristup teksturama
        /// </summary>
        private enum TextureObjects { Back = 0, Front, Bottom, Top, Left, Right, Carpet, Wood, CDFRONT, CDBACK };

        /// <summary>
        ///	 Broj tekstura.
        /// </summary>
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;
        
        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles =
        {
            "..//..//Textures//Sky-box//desertbk.png",
            "..//..//Textures//Sky-box//desertft.png",
            "..//..//Textures//Sky-box//desertdn.png",
            "..//..//Textures//Sky-box//desertup.png",
            "..//..//Textures//Sky-box//desertlf.png",
            "..//..//Textures//Sky-box//desertrt.png",
            "..//..//Textures//carpet.jpg",
            "..//..//Textures//wood.jpg",
            "..//..//Textures//cdft.jpg",
            "..//..//Textures//cdbk.jpg"
        };
        
        private float m_diskRotationY = 0.0f;

        private DispatcherTimer timer1;

        private DispatcherTimer timer2;

        private DispatcherTimer timer3;

        private DispatcherTimer timer4;

        private float m_cdTranslationZ = 0.0f;

        private float m_diskTranslationZ = 480.0f;

        private float m_diskRotationSpeed = 60.0f;

        private float m_diskTranslationY = 115.0f;

        private float m_computerX = 0.0f;

        private float m_computerZ = 0.0f;

        private float m_computerScalingFactor = 1.0f;

        private float m_spotlightRedAmbient = 0.4f;
        private float m_spotlightGreenAmbient = 0.0f;
        private float m_spotlightBlueAmbient = 0.0f;

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

        public bool LockControls { get; set; }

        public float ComputerX
        {
            get { return m_computerX; }
            set { m_computerX = value; }
        }

        public float ComputerZ
        {
            get { return m_computerZ; }
            set { m_computerZ = value; }
        }

        public float ComputerScalingFactor
        {
            get { return m_computerScalingFactor; }
            set { m_computerScalingFactor = value; }
        }

        public float SpotlightRedAmbient
        {
            get { return m_spotlightRedAmbient; }
            set { m_spotlightRedAmbient = value; }
        }

        public float SpotlightGreenAmbient
        {
            get { return m_spotlightGreenAmbient; }
            set { m_spotlightGreenAmbient = value; }
        }

        public float SpotlightBlueAmbient
        {
            get { return m_spotlightBlueAmbient; }
            set { m_spotlightBlueAmbient = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(string scenePath, int width, int height, OpenGL gl)
        {
            string sceneFileName = "Computer.obj";
            this.m_sceneComputer = new AssimpScene(Path.Combine(scenePath + "Computer\\"), sceneFileName, gl);
            sceneFileName = "CD.obj";
            this.m_sceneCD = new AssimpScene(Path.Combine(scenePath + "CD-ROM\\"), sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
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
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            m_sceneComputer.LoadScene();
            m_sceneComputer.Initialize();
            m_sceneCD.LoadScene();
            m_sceneCD.Initialize();
            SetupLighting(gl);
            InitializeAnimation();
            CreateTextures(gl);
        }

        /// <summary>
        ///  Kreiranje teksture.
        /// </summary>
        private void CreateTextures(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.GenTextures(m_textureCount, m_textures);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            for (int textureId = 0; textureId < m_textures.Length; textureId++)
            {
                Bitmap image = new Bitmap(m_textureFiles[textureId]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData bitmapdata = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[textureId]);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }

        /// <summary>
        /// Podesavanje osvetljenja
        /// </summary>
        private void SetupLighting(OpenGL gl)
        {
            float[] global_ambient = { 0.3f, 0.3f, 0.3f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            WhitePointLightSetup(gl);
            RedSpotlightSetup(gl);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LIGHT1);

            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        private void WhitePointLightSetup(OpenGL gl)
        {
            float[] light0ambient = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
        }

        private void RedSpotlightSetup(OpenGL gl)
        {
            float[] light1ambient = { m_spotlightRedAmbient, m_spotlightGreenAmbient, m_spotlightBlueAmbient, 1.0f };
            float[] light1diffuse = { 0.3f, 0.0f, 0.0f, 1.0f };
            float[] light1specular = { 0.8f, 0.0f, 0.0f, 1.0f };
            float[] light1direction = { 0.0f, -1.0f, 0.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light1direction);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION); // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)m_width / m_height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity(); // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
            
            gl.PushMatrix();
            gl.Translate(0.0f, 200.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            
            gl.LookAt(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f);
            DrawEnvironment(gl, 0.0f, 0.0f, 0.0f, 6000.0f, 6000.0f, 6000.0f);
            float[] light0pos = { 0.0f, 2500.0f, 0.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);

            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, 0.0f);

            gl.PushMatrix();
            float computerHeight = 575.0f;
            gl.Translate(m_computerX, computerHeight * m_computerScalingFactor / 2 - computerHeight / 2, m_computerZ);
            gl.Scale(m_computerScalingFactor, m_computerScalingFactor, m_computerScalingFactor);

            float[] light1pos = { -350.0f, 1400.0f, 0.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            RedSpotlightSetup(gl);
            DrawComputer(gl);
            DrawDisk(gl);
            gl.PopMatrix();

            DrawDesk(gl);
            gl.PopMatrix();

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            DrawSurface(gl);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            DrawText(gl);
            gl.PopMatrix();
            gl.Flush();
        }

        /// <summary>
        ///  Iscrtavanje teksturiranog kvadra - okruzenja.
        /// </summary>
        private void DrawEnvironment(OpenGL gl, float x, float y, float z, float width, float height, float length)
        {
            // BACK teksturu pridruzi zadnjoj stranici kocke
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Back]);

            // kocka je centrirana oko (x,y,z) tacke
            x = x - width / 2;
            y = y - height / 2;
            z = z - length / 2;

            // BACK stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(x + width, y, z);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(x + width, y + height, z);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(x, y + height, z);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(x, y, z);
            gl.End();

            // FRONT teksturu pridruzi prednjoj stranici kocke
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Front]);

            // FRONT stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(x, y, z + length);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(x, y + height, z + length);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(x + width, y + height, z + length);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(x + width, y, z + length);
            gl.End();

            // BOTTOM teksturu pridruzi donjoj stranici kocke
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Bottom]);

            // BOTTOM stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x, y, z);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x, y, z + length);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x + width, y, z + length);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x + width, y, z);
            gl.End();

            // TOP teksturu pridruzi gornjoj stranici
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Top]);

            // TOP stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x + width, y + height, z);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x + width, y + height, z + length);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x, y + height, z + length);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x, y + height, z);
            gl.End();

            // LEFT teksturu pridruzi levoj stranici
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Left]);

            // LEFT stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x, y + height, z);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x, y + height, z + length);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x, y, z + length);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x, y, z);
            gl.End();

            // RIGHT teksturu pridruzi desnoj stranici
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Right]);

            // RIGHT stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x + width, y, z);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x + width, y, z + length);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x + width, y + height, z + length);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x + width, y + height, z);
            gl.End();
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
            
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
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
            gl.Rotate(0f, m_diskRotationY, 0f);
            gl.Scale(50f, 50f, 50f);
            gl.Rotate(90f, 0f, 0f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.CDBACK]);
            gl.TexCoord(1.0f, 1.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Color(1f, 1f, 1f);
            Disk disk = new Disk();
            disk.TextureCoords = true;
            disk.Loops = 120;
            disk.Slices = 50;
            disk.InnerRadius = 0.2f;
            disk.OuterRadius = 1.5f;
            disk.CreateInContext(gl);
            disk.Render(gl, RenderMode.Render);
            
            gl.Rotate(180f, 0f, 0f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.CDFRONT]);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            m_diskRotationY += m_diskRotationSpeed;
            gl.Flush();
        }

        private void DrawSurface(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(0.0f, -878.0f, 0.0f);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            var carpetScale = 6.0f;
            gl.Scale(carpetScale, carpetScale, carpetScale);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Carpet]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(0.4f, 0.0f, 0.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(2000.0f, 0.0f, -2000f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-2000.0f, 0.0f, -2000.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-2000.0f, 0.0f, 2000f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(2000.0f, 0.0f, 2000f);
            gl.End();
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

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
