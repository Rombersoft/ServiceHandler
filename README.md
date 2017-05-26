# ServiceHelper
portable .net library for comfortable application managing from outside

It is created in order to easy make own service from your own application

USING EXAMPLE:


C# Code:
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

            
            
#Test on Linux:

nc 127.0.0.1 1983

command1



#Bash script for sending command to service:

#!/bin/sh

case $1 in
    command1) 
         mono --debug=mdb-optimizations /home/destructor/Monitor/WatchDog.exe &
         ;; 
    command2) 
            
         echo stop | nc 127.0.0.1 1101  
	 ;; 
     *) 
         echo " $0 Unknown argument $1"
         ;;
esac

      //*************************
      
Feedback and advices for improving send to rombersoft@gmail.com