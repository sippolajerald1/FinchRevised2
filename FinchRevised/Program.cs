using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinchAPI;
using System.IO;

namespace FinchRevised
{
    //********************************************
    //
    // Tite: Finch Control
    // Description: Revised
    // Application Type: Console
    // Author: Jerald Sippola
    // Date Created: 10-20-19
    // Last Modified: 10-26-19
    //
    //**********************************************

    class Program
    {
        public enum Command
        {
            NONE,
            MOVEFORWARD,
            MOVEBACKWARD,
            STOPMOTORS,
            WAIT,
            TURNRIGHT,
            TURNLEFT,
            LEDON,
            LEDOFF,
            TEMPERATURE,
            NOTEON,
            NOTEOFF,
            DONE
        }
        static void Main(string[] args)
        {
            SetTheme();
            DisplayWelcomeScreen();
            DisplayMainMenu();
            DisplayClosingScreen();
        }

        static void SetTheme()
        {
            string dataPath = @"Data\Theme.txt";

            string foreGroundColorString;
            ConsoleColor foreGroundColor;


            foreGroundColorString = File.ReadAllText(dataPath);
            Enum.TryParse(foreGroundColorString, out foreGroundColor);

            Console.ForegroundColor = foreGroundColor;

        }

        static void DisplayMainMenu()
        {
            //
            // Instatiate a Finch robot
            //

            Finch finchRobot = new Finch();

            bool finchRobotConnected = false;
            bool quitApplication = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Main Menu");
                //
                // get the users menu choice
                //
                Console.WriteLine("a) Connect Finch Robot");
                Console.WriteLine("b) Talent Show");
                Console.WriteLine("c) Data Recorder");
                Console.WriteLine("d) Alarm System");
                Console.WriteLine("e) User Programming");
                Console.WriteLine("f) Disconnect finch robot");
                Console.WriteLine("g) quit");
                Console.WriteLine("Enter choice: ");
                menuChoice = Console.ReadLine().ToLower();

                //
                // process users choice
                //
                switch (menuChoice)
                {
                    case "a":
                        finchRobotConnected = DisplayConnectFinchRobot(finchRobot);
                        break;
                    case "b":
                        if (finchRobotConnected)
                        {
                            DisplayTalentShow(finchRobot);

                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Please return to main menu and connect.");
                            DisplayContinuePrompt();
                        }

                        break;

                    case "c":
                        if (finchRobotConnected)
                        {
                            DisplayDataRecorder(finchRobot);
                        }
                        else
                        {
                            Console.WriteLine("The finch is not connected.");
                            DisplayContinuePrompt();
                        }
                        break;

                    case "d":
                        if (finchRobotConnected)
                        {
                            DisplayAlarmSystem(finchRobot);
                        }
                        else
                        {
                            Console.WriteLine("The Finch robot is not connected.");
                            DisplayContinuePrompt();
                        }
                        break;

                    case "e":
                        if (finchRobotConnected)
                        {
                            DisplayUserProgramming(finchRobot);

                        }
                        else
                        {
                            Console.WriteLine("The robot is not connected.");
                            DisplayContinuePrompt();

                        }
                        break;

                    case "f":
                        DisplayDisconnectFinchRobot(finchRobot);

                        break;
                    case "q":
                        quitApplication = true;
                        break;


                    default:
                        Console.WriteLine("\t******************************");
                        Console.WriteLine("\tPlease indicate your choice with a letter.");
                        Console.WriteLine("\t******************************");

                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitApplication);
        }

        #region USER PROGRAMMING

        static void DisplayUserProgramming(Finch finchRobot)
        {
            string menuChoice;
            bool quitApplication = false;

            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters;
            commandParameters.motorSpeed = 0;
            commandParameters.ledBrightness = 0;
            commandParameters.waitSeconds = 0;

            List<Command> commands = new List<Command>();

            do
            {
                DisplayScreenHeader("Main Menu");
                //
                // get the users menu choice
                //
                Console.WriteLine("a) Set Command Parameters");
                Console.WriteLine("b) Add commands.");
                Console.WriteLine("c) View commands");
                Console.WriteLine("d) Execute commands");
                Console.WriteLine("e) Write Commands to data file");
                Console.WriteLine("f) Read commands data file");
                Console.WriteLine("q) quit");
                Console.WriteLine("Enter choice: ");
                menuChoice = Console.ReadLine().ToLower();

                //
                //  process users choice
                //
                switch (menuChoice)
                {
                    case "a":
                        commandParameters = DisplayGetCommandParameters();
                        break;

                    case "b":
                        DisplayGetFinchCommands(commands);
                        break;

                    case "c":
                        displayFinchCommands(commands);
                        break;

                    case "d":
                        DisplayExecuteFinchCommands(finchRobot, commands, commandParameters);
                        break;

                    case "e":
                        DisplayWriteUserProgrammingData(commands);
                        break;

                    case "f":
                        commands = DisplayReadUserProgrammingData();
                        break;

                    case "q":
                        quitApplication = true;
                        break;


                    default:
                        Console.WriteLine("\t******************************");
                        Console.WriteLine("\tPlease indicate your choice with a letter.");
                        Console.WriteLine("\t******************************");

                        DisplayContinuePrompt();
                        break;
                }

            } while (!quitApplication);
        }

        static List<Command> DisplayReadUserProgrammingData()
        {
            string dataPath = @"Data\Data.txt";
            List<Command> commands = new List<Command>();
            string[] commandsString;

            DisplayScreenHeader("Load Commands from data file");

            Console.WriteLine("Ready to load commands from data file.");
            DisplayContinuePrompt();


            commandsString = File.ReadAllLines(dataPath);

            //
            // Create list of commend
            //


            Command command;

            foreach (string commandString in commandsString)
            {
                Enum.TryParse(commandString, out command);

                commands.Add(command);
            }




            Console.WriteLine();
            Console.WriteLine("Commands loaded successfully.");




            DisplayContinuePrompt();


            return commands;
        }

        static void DisplayWriteUserProgrammingData(List<Command> commands)
        {

            string dataPath = @"Data\Data.txt";
            List<string> commandsString = new List<string>();

            DisplayScreenHeader("Save commands to data file.");

            Console.WriteLine("Ready to save the commands to the data file.");
            DisplayContinuePrompt();

            //create list of command strings


            foreach (Command command in commands)
            {
                commandsString.Add(command.ToString());
            }

            File.WriteAllLines(dataPath, commandsString.ToArray());

            Console.WriteLine();
            Console.WriteLine("Commands successfully saved. ");



            DisplayContinuePrompt();
        }

        static void DisplayExecuteFinchCommands(
            Finch finchRobot,
            List<Command> commands,
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters)
        {
            int motorSpeed = commandParameters.motorSpeed;
            int ledBrightness = commandParameters.ledBrightness;
            int waitMilliSeconds = commandParameters.waitSeconds * 1000;


            DisplayScreenHeader("Execute Finch Commands");

            // info for and pause
            Console.ReadKey();


            foreach (Command command in commands)
            {
                switch (command)
                {
                    case Command.NONE:
                        break;
                    case Command.MOVEFORWARD:
                        finchRobot.setMotors(motorSpeed, motorSpeed);
                        break;
                    case Command.MOVEBACKWARD:
                        finchRobot.setMotors(-motorSpeed, -motorSpeed);
                        break;
                    case Command.STOPMOTORS:
                        finchRobot.setMotors(0, 0);
                        break;
                    case Command.WAIT: // echo 
                        finchRobot.wait(waitMilliSeconds);
                        break;
                    case Command.TURNRIGHT:
                        finchRobot.setMotors(0, motorSpeed);
                        break;
                    case Command.TURNLEFT:
                        finchRobot.setMotors(motorSpeed, 0);
                        break;
                    case Command.LEDON:
                        finchRobot.setLED(0, 20, 0);
                        break;
                    case Command.LEDOFF:
                        finchRobot.setLED(0, 0, 0);
                        break;
                    case Command.TEMPERATURE:
                        break;
                    case Command.NOTEON:
                        break;
                    case Command.NOTEOFF:
                        break;
                    case Command.DONE:
                        break;
                    default:
                        break;
                }
            }

            DisplayContinuePrompt();
        }

        static void displayFinchCommands(List<Command> commands)
        {
            DisplayScreenHeader("Finch Robot Commands");

            foreach (Command command in commands)
            {
                Console.WriteLine(command);
            }

            DisplayContinuePrompt();
        }

        static void DisplayGetFinchCommands(List<Command> commands)
        {
            Command command = Command.NONE;
            string userResponse;

            DisplayScreenHeader("Finch Robot Commands");

            // info for user

            while (command != Command.DONE)
            {
                Console.WriteLine("Enter command.");
                userResponse = Console.ReadLine().ToUpper();
                Enum.TryParse(userResponse, out command);


                if (command != Command.NONE)
                {
                    commands.Add(command);
                }

            }
            // ECHO COMMANDS

            DisplayContinuePrompt();
        }



        static (int motorSpeed, int ledBrightness, int waitSeconds)
            DisplayGetCommandParameters()
        {

            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters;

            commandParameters.motorSpeed = 0;
            commandParameters.ledBrightness = 0;
            commandParameters.waitSeconds = 0;

            // todo validate command parameters

            Console.Write("Enter Motor Speed [1-255]:"); // echo
            commandParameters.motorSpeed = int.Parse(Console.ReadLine());
            Console.WriteLine("You have enterred " + commandParameters.motorSpeed);

            // do not use echo
            // if ((commandParameters.motorSpeed >= 1) && (commandParameters.motorSpeed <= 255))
            //  {
            //      return commandParameters;
            //  }

            // todo validate command parameters
            Console.Write("Enter Led Brightness [1-255]:"); // echo
            commandParameters.ledBrightness = int.Parse(Console.ReadLine());
            Console.WriteLine("You have enterred " + commandParameters.ledBrightness);

            // if ((commandParameters.ledBrightness) >= 0 && (commandParameters.ledBrightness <= 255))


            Console.Write("Enter Wait in seconds:");
            commandParameters.waitSeconds = int.Parse(Console.ReadLine());
            Console.WriteLine("You have enterred wait for " + commandParameters.waitSeconds);

            // echo values to user
            return commandParameters;
        }

        #endregion

        #region ALARM SYSTEM

        static void DisplayAlarmSystem(Finch finchRobot)
        {

            string alarmType;
            int maxSeconds;
            double threshold;
            bool thresholdExceeded;

            DisplayScreenHeader("Alarm System: ");

            alarmType = DisplayGetAlarmType();
            maxSeconds = DisplayGetMaxSeconds();
            threshold = DisplayGetThreshold(finchRobot, alarmType);

            // pause and prompt the user

            thresholdExceeded = MonitorCurrentLightLevels(finchRobot, threshold, maxSeconds);

            if (thresholdExceeded)
            {
                if (alarmType == "light")
                {
                    Console.WriteLine("Maximum Light level exceeded");
                }
                else
                {
                    Console.WriteLine("Maximum Temperature Level Exceeded");
                }
            }
            else
            {
                Console.WriteLine("Maximum Time Exceeded");
            }

            DisplayMainMenu();
        }

        static bool MonitorCurrentLightLevels(Finch finchRobot, double threshold, int maxSeconds)
        {
            bool thresholdExceeded = false;
            int currentLightLevel;
            double seconds = 0;

            while (!thresholdExceeded && seconds <= maxSeconds)
            {
                currentLightLevel = finchRobot.getLeftLightSensor();

                DisplayScreenHeader("Monitor Light Levels");
                Console.WriteLine($"Maximum Light Level: {threshold}");
                Console.WriteLine($"Current Light Level: {currentLightLevel}");

                if (currentLightLevel > threshold) thresholdExceeded = true;

                finchRobot.wait(500);
                seconds += 0.5;
            }
            return thresholdExceeded;
        }

        static double DisplayGetThreshold(Finch finchRobot, string alarmType)
        {
            double threshold = 0;
            DisplayScreenHeader("Threshold Value");

            switch (alarmType)
            {
                case "light":
                    Console.Write($"Current LightLevel: {finchRobot.getLeftLightSensor()}");
                    Console.WriteLine();
                    Console.Write($"Enter maximum light level.[0-255]:");
                    threshold = double.Parse(Console.ReadLine()); // todo validate!!!
                    break;

                case "temperature":
                    break;

                default:

                    break;
            }

            DisplayContinuePrompt();
            return threshold;
        }

        static int DisplayGetMaxSeconds()
        {
            // todo - validate user response

            Console.Write("Enter maximum number of seconds:");
            return int.Parse(Console.ReadLine());
        }

        static string DisplayGetAlarmType()
        {
            //validate user response!!!

            Console.Write("Enter alarm type [light or temperature]:");
            return Console.ReadLine();
        }

        #endregion

        #region DATA RECORDER

        static void DisplayDataRecorder(Finch finchRobot)
        {
            double frequency;
            int numberOfDataPoints;

            DisplayScreenHeader("Data Recorder: ");



            // give user info about what is going on

            frequency = DisplayGetDataPointFrequency(finchRobot);
            numberOfDataPoints = DisplayGetNumberOfDataPoints(finchRobot);

            //
            // instantiate (create) array
            //

            double[] temperatures = new double[numberOfDataPoints];

            // warn user before recordings

            DisplayGetDataReadings(numberOfDataPoints, frequency, temperatures, finchRobot);
            DisplayDataRecorderData(temperatures);

            DisplayMainMenu();
        }

        static void DisplayDataRecorderData(double[] temperatures)
        {
            DisplayScreenHeader("Temperatures");

            // provide some info to user

            Console.WriteLine("Data Set: ");
            Console.WriteLine();

            for (int index = 0; index < temperatures.Length; index++)
            {
                Console.WriteLine($"Temperature {index + 1}: {temperatures[index]:F2}");
            }

            DisplayContinuePrompt();
        }

        static void DisplayGetDataReadings(int numberOfDataPoints,
        double frequencyOfDataPoints,
        double[] temperatures,
        Finch finchRobot)
        {

            DisplayScreenHeader("Get Temperatures");

            // give user info and a prompt

            DisplayContinuePrompt();

            //
            // get temperatures
            //

            for (int index = 0; index < numberOfDataPoints; index++)
            {
                temperatures[index] = finchRobot.getTemperature();
                int milliSeconds = (int)(frequencyOfDataPoints * 1000);
                finchRobot.wait(milliSeconds);

                Console.WriteLine($"Temperature {index + 1}, {temperatures[index]:F2}");
            }

            DisplayContinuePrompt();
        }

        static double DisplayGetDataPointFrequency(Finch finchRobot)
        {
            double frequency;
            // userResponse;

            DisplayScreenHeader("Get Frequency of Recordings");

            Console.Write("Enter Frequency of Recordings.");
            // userResponse = Console.ReadLine();
            // double.TryParse(userResponse, out dataPointFrequency);
            double.TryParse(Console.ReadLine(), out frequency);

            DisplayMainMenuPrompt();

            return frequency;
        }

        static int DisplayGetNumberOfDataPoints(Finch finchRobot)
        {
            int numberOfDataPoints;

            DisplayScreenHeader("Number Of Data Points");
            Console.Write("Enter the number of data points.");
            int.TryParse(Console.ReadLine(), out numberOfDataPoints);

            DisplayMainMenuPrompt();
            return numberOfDataPoints;
        }

        #endregion

        #region TALENT SHOW

        static void DisplayTalentShow(Finch finchRobot)
        {
            DisplayScreenHeader("Talent Show");

            Console.WriteLine("The finch robot is ready to show its talents.");
            DisplayContinuePrompt();

            finchRobot.noteOn(1000);
            finchRobot.setLED(0, 200, 20);
            finchRobot.setMotors(100, 100);
            finchRobot.wait(1000);
            finchRobot.noteOff();
            finchRobot.wait(1000);
            finchRobot.noteOn(1000);
            finchRobot.setLED(200, 0, 0);
            finchRobot.setMotors(0, 0);
            finchRobot.noteOff();
            finchRobot.noteOn(2000);
            finchRobot.setMotors(-100, 100);
            finchRobot.setLED(0, 0, 200);
            finchRobot.setMotors(-100, -100);
            finchRobot.noteOn(5000);
            finchRobot.wait(1000);
            finchRobot.noteOn(5000);
            finchRobot.setMotors(0, 0);
            finchRobot.noteOn(7500);
            finchRobot.wait(2000);
            finchRobot.noteOn(5000);
            finchRobot.setMotors(100, 0);
            finchRobot.wait(1000);
            finchRobot.setMotors(-100, 100);
            finchRobot.wait(2000);
            finchRobot.setLED(250, 0, 0);
            finchRobot.setMotors(0, 0);
            finchRobot.noteOn(1000);
            finchRobot.wait(5000);
            finchRobot.setLED(0, 200, 0);
            finchRobot.noteOff();

            for (int lightLevel = 0; lightLevel < 255; lightLevel++)
            {
                finchRobot.setLED(lightLevel, lightLevel, lightLevel);

            }

            DisplayContinuePrompt();
        }


        #endregion

        #region FINCH ROBOT MANAGEMENT


        static void DisplayDisconnectFinchRobot(Finch finchRobot)
        {
            DisplayScreenHeader("Disconnect Finch Robot");

            Console.WriteLine("Ready to disconnect finch robot.");
            DisplayContinuePrompt();

            finchRobot.disConnect();

            Console.WriteLine("Finch robot is now disconnected.");
            DisplayContinuePrompt();
        }

        static bool DisplayConnectFinchRobot(Finch finchRobot)
        {
            bool finchRobotConnected;

            DisplayScreenHeader("Connect Finch Robot");

            Console.WriteLine("Ready to connect to the finch robot. Please be sure to connect the USB robot to computer.");
            DisplayContinuePrompt();

            finchRobotConnected = finchRobot.connect();

            if (finchRobotConnected)
            {

                finchRobot.setLED(0, 255, 0);
                finchRobot.noteOn(15000);
                finchRobot.wait(1000);
                finchRobot.noteOff();

                Console.WriteLine("Finch robot is now connected.");

            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Unable to connect to the finch robot"); ;
            }

            DisplayContinuePrompt();
            return finchRobotConnected;
        }

        #endregion

        #region USER INTERFACE

        /// <summary>
        /// display welcome screen
        /// <summary>

        static void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tFinch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display closing screen
        /// </summary>

        static void DisplayClosingScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// Helper Methods
        /// </summary>

        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static void DisplayMainMenuPrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return to main Menu.");
            Console.ReadKey();
        }

        ///<summary>
        /// display screen header
        /// </summary>             

        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }

        #endregion
    }

}