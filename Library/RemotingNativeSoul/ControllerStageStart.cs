﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Remoting.Soul.Native
{
    class StageStart : Regulus.Utility.IStage
    {
        public event Action<Regulus.Remoting.ICore, int, float> DoneEvent;

        Regulus.Utility.Command _Command;
        Regulus.Utility.Console.IViewer _View;
        public StageStart(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view)
        {
            _View = view;
            _Command = command;
        }
        void Utility.IStage.Enter()
        {
            _Command.Register<int, string, string>("Launch", _Start);
            _Command.Register<string>("LaunchIni", _StartIni);

            
            _View.WriteLine("======Ini file format description=====");
            _View.WriteLine("Example.");
            _View.WriteLine("[Launch]");
            _View.WriteLine("port = 12345");
            _View.WriteLine("path = game.dll");
            _View.WriteLine("class = Company.Project.Center");
            _View.WriteLine("======================================");
        }

        private void _StartIni(string path)
        {
            var ini = new Regulus.Utility.Ini(System.IO.File.ReadAllText(path));
            var port_string = ini.Read("Launch", "port");
            int port = int.Parse(port_string);
            string dllpath = ini.Read("Launch", "path");
            string className = ini.Read("Launch", "class");

            _Start(port, dllpath, className);
        }

        private void _Start(int port, string path, string class_name)
        {



            var stream = System.IO.File.ReadAllBytes(path);

            try 
            {
                var core = Regulus.Utility.Loader.Load(stream, class_name);
                DoneEvent(core, port, 0);
            }
            catch (SystemException ex)
            {
                _View.WriteLine(ex.ToString());
            }
            


            
        }

        /*private void _LoadLibrary(string work_dir)
        {
                
            var files = from f in System.IO.Directory.EnumerateFiles(work_dir, "*.dll", System.IO.SearchOption.AllDirectories) select f;
            foreach(var file in files)
            {
                _View.Write("載入程式庫 " + file + "...");
                try
                {
                    var assembly = System.Reflection.Assembly.LoadFile(file);                        
                    _View.WriteLine("完成!");
                }
                catch (SystemException ex)
                {
                    _View.WriteLine("失敗!" + ex.ToString());
                }
                    
            }
        }*/

        void Utility.IStage.Leave()
        {
            _Command.Unregister("Launch");
            _Command.Unregister("LaunchIni");
        }

        void Utility.IStage.Update()
        {

        }
    }
}
