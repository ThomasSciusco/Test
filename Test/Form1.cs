using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        interface IDomanda
        {
            string OttieniDomanda();
            bool VerificaRisposta(string rispostaUtente);
        }

        class DomandaBase : IDomanda
        {
            protected string domanda;
            protected string rispostaCorretta;

            public DomandaBase(string domanda, string rispostaCorretta)
            {
                this.domanda = domanda;
                this.rispostaCorretta = rispostaCorretta;
            }

            public virtual string OttieniDomanda()
            {
                return domanda;
            }

            public virtual bool VerificaRisposta(string rispostaUtente)
            {
                return rispostaUtente.Equals(rispostaCorretta, StringComparison.OrdinalIgnoreCase);
            }
        }

        class DomandaSceltaMultipla : DomandaBase
        {
            private List<string> opzioni;

            public DomandaSceltaMultipla(string domanda, List<string> opzioni, string rispostaCorretta)
                : base(domanda, rispostaCorretta)
            {
                this.opzioni = opzioni;
            }

            public override string OttieniDomanda()
            {
                string testoDomanda = base.OttieniDomanda() + "\n";
                for (int i = 0; i < opzioni.Count; i++)
                {
                    testoDomanda += $"{i + 1}. {opzioni[i]}\n";
                }
                return testoDomanda;
            }
        }

        class DomandaComposta : IDomanda
        {
            private List<IDomanda> domande = new List<IDomanda>();

            public void AggiungiDomanda(IDomanda domanda)
            {
                domande.Add(domanda);
            }

            public string OttieniDomanda()
            {
                string domandaComposta = "Domanda Composta:\n";
                foreach (var domanda in domande)
                {
                    domandaComposta += domanda.OttieniDomanda() + "\n";
                }
                return domandaComposta;
            }

            public bool VerificaRisposta(string rispostaUtente)
            {
                bool tutteCorrette = true;
                foreach (var domanda in domande)
                {
                    tutteCorrette &= domanda.VerificaRisposta(rispostaUtente);
                }
                return tutteCorrette;
            }
        }

        public partial class MainForm : Form
        {
            private List<IDomanda> domandeTest;

            public MainForm()
            {
                InitializeComponent();
                InizializzaTest();
            }

            private void InizializzaTest()
            {
                domandeTest = new List<IDomanda>();

                var domandaSingolaScelta = new DomandaBase("Qual è la capitale dell'Italia?", "Roma");
                var domandaSceltaMultipla = new DomandaSceltaMultipla("Quali di seguito sono colori primari?",
                    new List<string> { "Rosso", "Blu", "Verde", "Giallo" }, "Rosso");
                var domandaVeroFalso = new DomandaBase("Il Sole è una stella?", "Si");

                var domandaComposta = new DomandaComposta();
                domandaComposta.AggiungiDomanda(domandaSingolaScelta);
                domandaComposta.AggiungiDomanda(domandaSceltaMultipla);
                domandaComposta.AggiungiDomanda(domandaVeroFalso);

                domandeTest.Add(domandaComposta);
            }

            private void AvviaTestButton_Click(object sender, EventArgs e)
            {
                int risposteCorrette = 0;

                foreach (var domanda in domandeTest)
                {
                    string rispostaUtente = OttieniRispostaUtente(domanda.OttieniDomanda());
                    if (domanda.VerificaRisposta(rispostaUtente))
                    {
                        risposteCorrette++;
                    }
                }

                double percentuale = (double)risposteCorrette / domandeTest.Count * 100;
                MessageBox.Show($"Punteggio: {percentuale}%");
            }

            private string OttieniRispostaUtente(string domanda)
            {
                return MessageBox.Show(domanda, "Domanda", MessageBoxButtons.OKCancel) == DialogResult.OK ? "Si" : "No";
            }
        }


    }
}

