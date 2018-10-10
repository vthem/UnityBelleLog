using System;
using System.Collections;
using UnityEngine;
using Zob;

public class RuntimeLogGenerator : MonoBehaviour {



    private LogGenerator _generator = new LogGenerator();

    private void Update()
    {
        _generator.Update();
    }

    private class LogGenerator
    {
        private static readonly string[] _words = new string[] { "Miserando", "cuiusdam", "conculcans", "pugnis", "addictum", "furoris", "audaciam", "vulgi", "ingravesceret", "ut", "incessens", "seminecem", "suos", "miserando", "rectoremque", "furoris", "miserando", "penuria", "calcibus", "ut", "conculcans", "imaginem", "sui", "penuria", "in", "famis", "interitum", "vulgi", "iudicio", "periculi", "iudicio", "vulgi", "sibi", "rectoremque", "clari", "ignibus", "exitio", "seminecem", "recenti", "miserando", "considerans", "pugnis", "ut", "similia", "cum", "incessens", "rectoremque", "ingravesceret", "considerans", "documento", "furoris", "sibi", "commeatuum", "quod", "audaciam", "laniatu", "ut", "Eubuli", "quisque", "iudicio", "pugnis", "unius", "miserando", "similia", "sui", "discerpsit", "pugnis", "cum", "et", "vulgi", "unius", "penuria", "suos", "cuius", "cuiusdam", "post", "documento", "quod", "cuiusdam", "subditis", "inpulsu", "ut", "calcibus", "discerpsit", "imperiali", "unius", "cuius", "inflammavit", "formidabat", "Eubuli", "Eubuli", "ambitiosam", "et", "seminecem", "subditis", "quisque", "famis", "formidabat", "formidabat", "exitio", "Adscisceret", "Scutariorum", "videre", "saepius", "frater", "Arctoae", "gestum", "adscisceret", "gestum", "quoque", "per", "frater", "Arctoae", "per", "laborum", "ut", "quod", "inprudentiam", "Scutariorum", "et", "inprudentiam", "saepius", "ingenii", "Arctoae", "futurum", "flagrantibus", "videre", "cuperet", "gestum", "quoque", "opifex", "laborum", "vultu", "quod", "multos", "participemque", "solus", "callidus", "diu", "eum", "clemens", "laborum", "Scutariorum", "et", "flagrantibus", "solus", "quoque", "maiestatis", "eum", "adscisceret", "ingenii", "inprudentiam", "vultu", "saepius", "frater", "Scudilo", "est", "et", "et", "eum", "Arctoae", "et", "seriis", "persuasionis", "Scudilo", "qui", "solus", "seriis", "fessae", "saepius", "suae", "ut", "per", "clemens", "laborum", "socium", "participemque", "siquid", "replicando", "pellexit", "saepius", "maiestatis", "vultu", "per", "maiestatis", "persuasionis", "remissurus", "maiestatis", "futurum", "subagrestis", "Scutariorum", "suae", "flagrantibus", "et", "eum", "callidus", "vultu", "siquid", "quos", "quos", "Nec", "vero", "praeter", "nascitur", "caelibes", "extra", "orbos", "potest", "liberis", "liberis", "quicquid", "praeter", "caelibes", "orbos", "praeter", "esse", "quorundam", "homines", "sine", "Romae", "vile", "et", "vile", "obsequiorum", "obsequiorum", "orbos", "aestimant", "nascitur", "et", "obsequiorum", "orbos", "obsequiorum", "quicquid", "inanes", "nec", "caelibes", "inanes", "praeter", "aestimant", "quorundam", "sine", "et", "et", "homines", "potest", "flatus", "Romae", "flatus", "homines", "quicquid", "praeter", "liberis", "obsequiorum", "flatus", "homines", "Romae", "flatus", "nascitur", "Romae", "caelibes", "sine", "vero", "esse", "sine", "orbos", "orbos", "caelibes", "praeter", "diversitate", "esse", "potest", "diversitate", "aestimant", "qua", "extra", "pomerium", "liberis", "Romae", "caelibes", "quicquid", "orbos", "sine", "diversitate", "credi", "nec", "nec", "aestimant", "sine", "Romae", "nascitur", "flatus", "pomerium", "obsequiorum", "pomerium", "aestimant", "quorundam", "obsequiorum", "vile", "caelibes", "vile", "Constantius", "reserato", "consulatu", "reserato", "terrae", "Caesaris", "reserato", "Gundomadum", "oriens", "Constantius", "crebris", "confines", "perferret", "terrae", "vastabantur", "Caesaris", "suo", "excursibus", "crebris", "excursibus", "excursibus", "vastabantur", "petit", "quorum", "reserato", "limitibus", "tepore", "Alamannorum", "quorum", "consulatu", "tepore", "oriens", "quorum", "Constantius", "Valentiam", "excursibus", "fratres", "reges", "tepore", "arma", "Alamannorum", "reges", "vastabantur", "Constantius", "reserato", "limitibus", "terrae", "perferret", "Valentiam", "confines", "diu", "Valentiam", "septies", "Valentiam", "tepore", "consulatu", "Valentiam", "confines", "reges", "septies", "vastabantur", "oriens", "quorum", "Caesaris", "quorum", "dum", "Valentiam", "Alamannorum", "et", "oriens", "et", "Gundomadum", "septies", "consulatu", "diu", "Gallorum", "septies", "confines", "Gallorum", "septies", "ter", "suo", "et", "limitibus", "suo", "excursibus", "Caesaris", "dum", "Gundomadum", "fratres", "excursibus", "reserato", "et", "Vadomarium", "reserato", "terrae", "diu", "in", "fratres", "diu" };

        private Zob.Logger _logger;
        private DateTime _nextUpdate = DateTime.Now;
        private System.Random _rand = new System.Random();

        public LogGenerator()
        {
            _logger = new Zob.Logger("random");
        }

        public void Update()
        {
            if (DateTime.Now < _nextUpdate)
            {
                return;
            }
            _nextUpdate = DateTime.Now + TimeSpan.FromSeconds(_rand.NextDouble() * 2.0);

            var randomLevel = (LogLevel)UnityEngine.Random.Range((int)LogLevel.Trace, (int)(LogLevel.Fatal) + 1);
            var wordCount = _rand.Next(1, 10);
            string line = string.Empty;
            for (int i = 0; i < wordCount; ++i)
            {
                line = line + _words[_rand.Next(0, _words.Length)] + " ";
            }
            switch (randomLevel)
            {
                case LogLevel.Trace:
                _logger.Trace();
                break;
                case LogLevel.Debug:
                _logger.Debug(line);
                break;
                case LogLevel.Info:
                _logger.Info(line);
                break;
                case LogLevel.Warning:
                _logger.Warning(line);
                break;
                case LogLevel.Error:
                _logger.Error(line);
                break;
                case LogLevel.Fatal:
                _logger.Fatal(line);
                break;
                default:
                throw new System.Exception("unsupported random log level");
            }
        }
    }

}
