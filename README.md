# ServiceHandler
portable .net library for comfortable application managing from outside

It is created in order to easy make own service from your own application

USING EXAMPLE:

            ServiceSocket ss = new ServiceSocket(1983, false);
            ss.OnCommand += (string command) =>
            {
                Console.WriteLine("There is new command: {0}", command);
		switch(command)
		{
			case "command1":
				//some actions
				break;
			case "command2":
				//some actions
				break;
		}
            };
            ss.StartListen();
            Console.ReadKey();
            ss.StopListen();
            ss.Dispose();
            Console.ReadKey();

Feedback and advices for improving send to rombersoft@gmail.com