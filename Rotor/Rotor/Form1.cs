using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rotor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public int[,] Polja = new int[4, 4];
        public const int Xp = 0; // puzzle starts at x=0
        public const int Yp = 0; // puzzle starts at y=0

        PictureBox Plocica = new PictureBox();

        private void Button1_Click(object sender, EventArgs e)
        {
            byte I = 0;
            int J = 0;
            int K = 0;
            int[] Niz;
            Niz = new int[16];
            Random RndNum = new Random();

            // formirj niz od 16 elemenata
            for (I = 0; I <= 15; I++)
            {
                Opet:
                J = RndNum.Next(1, 17); // izaberi slučajan broj između 0 i 17
                for (K = 0; K <= I; K++)
                {
                    if (Niz[K] == J) // proveri da li među već izvučenima postoji broj J
                    {
                        goto Opet; // ako postoji izvici sledeći
                    }
                }
                Niz[I] = J; // korektan broj postavi u niz
            }

            // Popuni matricu 4x4 sa brojevima iz niza
            for (I = 0; I <= 3; I++)
            {
                for (J = 0; J <= 3; J++)
                {
                    Polja[I, J] = Niz[I * 4 + J];
                    // Plocica je kontrola pictureBox i u objektu this su smešteni na sledeći način
                    // pictureBox16=Controls[1]
                    // pictureBox15=Controls[2]...
                    // pictureBox1=Controls[16]
                    Plocica = (PictureBox)this.Controls[17 - Polja[I, J]];
                    Plocica.Location = new Point(I * 72 + Xp, J * 72 + Yp);
                }
            }
        }

        // Kursor treba da menja oblik u zavisnosti gde miš stoji iznad pločice

        private Cursor TakeShape(int x, int y)
        {
            if (x > y)
            {
                // Gornja zona je PanNorth
                if (71 - x > y)
                {
                    return Cursors.PanNorth;
                }
                // Desna zona je PanEast
                else
                {
                    return Cursors.PanEast;
                }
            }
            else
            {
                // Donja zona je PanSouth
                if (71 - x < y)
                {
                    return Cursors.PanSouth;
                }
                // Leva zona je PanWest
                else
                {
                    return Cursors.PanWest;
                }
            }
        }

        public byte Koja;
        public string Kuda;

        private byte GdeSi(int XK, int YK)
        {
            byte XTemp;
            byte YTemp;

            XTemp = (byte)Math.Floor((XK - Xp) / 72.0);
            YTemp = (byte)Math.Floor((YK - Yp) / 72.0);

            // vraćam poziciju puzzle u obliku bajta i stavljam kolonu u Hi byte a vrstu u Lo byte
            XTemp <<= 4;
            return (byte)(XTemp + YTemp);
        }

        private enum Kretanja
        {
            Dole,
            Gore,
            Levo,
            Desno,
            Red,
            Kolona
        };

        private const Kretanja rtPomeriDole = Kretanja.Dole;
        private const Kretanja rtPomeriGore = Kretanja.Gore;
        private const Kretanja rtPomeriDesno = Kretanja.Desno;
        private const Kretanja rtPomeriLevo = Kretanja.Levo;
        private const Kretanja rtPomeriKolonu = Kretanja.Kolona;
        private const Kretanja rtPomeriRed = Kretanja.Red;

        private void Rotiraj(byte Kolona, byte Red, Cursor Kuda, bool Prikaz)
        {
            byte J;
            // rotiraj kolonu na gore
            if (Kuda == Cursors.PanNorth)
            {
                J = (byte)Polja[Kolona, 0];
                // najpre ih vizelno rotiraj ...
                // pozovi proceduru za vizuelnu rotaciju
                // proceduri se šalje podatak šta se rotira (kolona ili red)
                //                            koja se rotira (redni broj kolone ili reda)
                //                            kuda se rotira (gore - dole - levo - desno)
                //                            koji broj se nalazi na pomoćnoj pločici
                if (Prikaz) { Prikazi(rtPomeriKolonu, Kolona, rtPomeriGore, J); }
                // ... pa to sprovedi i na matrici.
                for (int I = 1; I <= 3; I++)
                {
                    Polja[Kolona, I - 1] = Polja[Kolona, I];
                }
                Polja[Kolona, 3] = J;
            }
            // rotiraj kolonu na dole
            if (Kuda == Cursors.PanSouth)
            {
                J = (byte)Polja[Kolona, 3];
                if (Prikaz) { Prikazi(rtPomeriKolonu, Kolona, rtPomeriDole, J); }
                for (int I = 2; I >= 0; I--)
                {
                    Polja[Kolona, I + 1] = Polja[Kolona, I];
                }
                Polja[Kolona, 0] = J;
            }
            // rotiraj red na levo
            if (Kuda == Cursors.PanWest)
            {
                J = (byte)Polja[0, Red];
                if (Prikaz) { Prikazi(rtPomeriRed, Red, rtPomeriLevo, J); }
                for (int I = 1; I <= 3; I++)
                {
                    Polja[I - 1, Red] = Polja[I, Red];
                }
                Polja[3, Red] = J;
            }
            // rotiraj red na desno
            if (Kuda == Cursors.PanEast)
            {
                J = (byte)Polja[3, Red];
                if (Prikaz) { Prikazi(rtPomeriRed, Red, rtPomeriDesno, J); }
                for (int I = 2; I >= 0; I--)
                {
                    Polja[I + 1, Red] = Polja[I, Red];
                }
                Polja[0, Red] = J;
            }
        }

        private void Prikazi(Kretanja Koga, int Koji, Kretanja Gde, int Šta)
        {
            // donje dve promenljive čuvaju koordinate pomoćne pločice
            int TempI = 0;
            int TempJ = 0;

            if (Koga == rtPomeriKolonu)
            {
                TempI = Koji * 72;
                if (Gde == rtPomeriDole)
                {
                    TempJ = -72;
                }
                else
                {
                    TempJ = 4 * 72;
                }
            }
            if (Koga == rtPomeriRed)
            {
                TempJ = Koji * 72;
                if (Gde == rtPomeriDesno)
                {
                    TempI = -72;
                }
                else
                {
                    TempI = 4 * 72;
                }
            }

            // pomoćnu pločicu postavi u zavisnoti šta se i kuda se rotira. Koordinate se određuju iz gornjih if then uslova
            pictureBoxTemp.Location = new Point(TempI + Xp, TempJ + Yp);
            // na pomoćnu pločicu postavi sliku broja koji ispada i ponovo se pojavljuje pri rotaciji
            Plocica = (PictureBox)this.Controls[17 - Šta];
            pictureBoxTemp.Image = Plocica.Image;

            int shGD = 0;
            int shLD = 0;
            int rpt = 0;
            int tmpV;
            int I = 0;
            int J = 0;
            bool rfr = false;

            // sprovedi glatko klizanje pločica
            // shGD je šift gore-dole, a shLD je šift levo-desno
            int StepForShift = 3;
            do
            {
                if (Koga == rtPomeriKolonu & Gde == rtPomeriGore) { shGD = shGD - StepForShift; }
                if (Koga == rtPomeriKolonu & Gde == rtPomeriDole) { shGD = shGD + StepForShift; }
                if (Koga == rtPomeriRed & Gde == rtPomeriLevo) { shLD = shLD - StepForShift; }
                if (Koga == rtPomeriRed & Gde == rtPomeriDesno) { shLD = shLD + StepForShift; }
                // povećaj brojač ponavljanja sve do 72. Rotacija se obavlja glatko, piksel po piksel.
                rpt = rpt + StepForShift;

                // pomeri četiri pločice ...
                for (int KR = 0; KR <= 3; KR++)
                {
                    if (Koga == rtPomeriRed)
                    {
                        tmpV = Polja[KR, Koji];
                        I = KR;
                        J = Koji;
                    }
                    else
                    {
                        tmpV = Polja[Koji, KR];
                        I = Koji;
                        J = KR;
                    }
                    Plocica = (PictureBox)this.Controls[17 - tmpV];
                    Plocica.Location = new Point(I * 72 + Xp + shLD, J * 72 + Yp + shGD);

                }
                // pomeri pomoćnu pločicu
                pictureBoxTemp.Location = new Point(TempI + Xp + shLD, TempJ + Yp + shGD);

                //refrešuj svako drugo pomeranje da bi se igrica ubrzala
                if (rfr) this.Refresh();
                {
                    rfr = !rfr;
                }
            } while (rpt != 72);

            // Postavi stvarnu pločicu koja je istisnuta pomeranjem na mesto privremene
            Plocica = (PictureBox)this.Controls[17 - Šta];
            Plocica.Location = pictureBoxTemp.Location;
        }

        private void ObradiKlik(object ulaz)
        {
            byte Lokacija;
            byte Kolona;
            byte Red;
            PictureBox pločica = (PictureBox)ulaz;
            // donja promenljiva sadži položaj kursora prilikom klika.
            // Kolona je smeštena u bitovima 7,6,5,4
            // A red je smešten u bitovima 3,2,1,0 
            Lokacija = GdeSi(pločica.Location.X, pločica.Location.Y);
            Kolona = (byte)(Lokacija >> 4);
            Red = (byte)(Lokacija & 15);
            // pictureBox.Cursor će pokazati kuda da se rotira
            Rotiraj(Kolona, Red, pločica.Cursor, true);
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // takeShape is function which returns cursor shape. See above
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox4_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox5_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox5_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox6_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox7_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox7_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox8_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox8_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox9_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox9_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox10_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox10_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox11_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox11_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox12_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox12_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox13_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox13_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox14_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox14_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox15_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox15_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void PictureBox16_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = TakeShape(e.X, e.Y);
        }

        private void PictureBox16_Click(object sender, EventArgs e)
        {
            ObradiKlik(sender);
        }

        private void Button1_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }
    }
}