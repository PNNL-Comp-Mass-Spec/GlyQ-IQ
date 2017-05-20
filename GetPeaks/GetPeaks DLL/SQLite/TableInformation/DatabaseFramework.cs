using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;

namespace GetPeaks_DLL.SQLite.TableInformation
{
    public static class DatabaseFramework
    {
        public static bool Create(string fileName, Object databaseLock, List<string> columnList)
        {
            //List<string> columnList = new List<string>();
            //columnList.Add(columnName);


            bool didThisWork = false;

            string connectionString = "";
            string commandTextString = "";
            connectionString = "Data Source=" + fileName;
            int counter = 0;
            //Profiler newProfiler = new Profiler();
            int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
            //newProfiler.printMemory("Before Lock " + threadName);



            //System.Threading.Monitor.TryEnter(databaseLock, 15);
            lock (databaseLock)
            {
                //newProfiler.printMemory(counter.ToString()); counter++;
                //newProfiler.printMemory(counter.ToString()); counter++;
                try
                {
                    //newProfiler.printMemory(counter.ToString()); counter++;
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        //newProfiler.printMemory(counter.ToString()); counter++;
                        try
                        {
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            connection.Open();
                            //newProfiler.printMemory(counter.ToString()); counter++;
                        }
                        catch (SQLiteException exception)
                        {
                            #region inside catch
                            if (exception.Message == "The database file is locked\r\ndatabase is locked")
                            {
                                Console.WriteLine("Failed :" + exception.Message + "  Now what do we do");
                                Console.ReadKey();
                            }
                            didThisWork = false;
                            Console.WriteLine("Failed :" + exception.Message);
                            Console.ReadKey();
                            #endregion
                        }
                        catch (Exception anything)
                        {
                            Console.WriteLine("Failed :" + anything.Message + "  Now what do we do");
                        }
                        //newProfiler.printMemory(counter.ToString()); counter++;
                        using (SQLiteCommand command = new SQLiteCommand())
                        {
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            command.Connection = connection;
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            #region set command for column names
                            //command.CommandText = "CREATE TABLE IF NOT EXISTS FeatureLiteTable (ID double, Abundance double, ChargeState double, DriftTime double, MonoisotopicMass double, RetentionTime double , Score double);";
                            commandTextString = "CREATE TABLE IF NOT EXISTS TableInfo (";
                            for (int i = 0; i < columnList.Count - 1; i++)
                            {
                                commandTextString += columnList[i] + " double,";
                            }
                            commandTextString += columnList[columnList.Count - 1] + " double);";//last point
                            #endregion
                            command.CommandText = commandTextString;

                            try
                            {
                                #region inside try
                                command.ExecuteNonQuery();
                                //newProfiler.printMemory(counter.ToString()); counter++;
                                using (SQLiteTransaction transaction = connection.BeginTransaction())
                                {
                                    //newProfiler.printMemory(counter.ToString()+"L"); counter++;
                                    #region set command string for inserts
                                    //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                                    commandTextString = "INSERT INTO TableInfo values (";
                                    for (int i = 0; i < columnList.Count - 1; i++)
                                    {
                                        commandTextString += "@" + columnList[i] + ",";
                                    }
                                    commandTextString += "@" + columnList[columnList.Count - 1] + ")";

                                    #endregion

                                    command.CommandText = commandTextString;

                                    #region setup columns and add data to each column.  In this case initialize count

                                    for (int i = 0; i < columnList.Count; i++)
                                    {
                                        DbParameter parameterNumber0 = command.CreateParameter();
                                        parameterNumber0.ParameterName = columnList[i];
                                        parameterNumber0.DbType = DbType.Int32;
                                        parameterNumber0.Value = 0; //sets the count to zero

                                        command.Parameters.Add(parameterNumber0);
                                    }
                                    command.ExecuteNonQuery();
                                    #endregion

                                    #region commit try catch
                                    try
                                    {
                                        //newProfiler.printMemory(counter.ToString() + "y"); counter++;
                                        transaction.Commit();
                                        //newProfiler.printMemory(counter.ToString() + "z"); counter++;
                                        //newProfiler.printMemory(counter.ToString() + "z2"); counter++;
                                    }
                                    catch (SQLiteException exception)
                                    {
                                        #region inside catch
                                        if (exception.Message == "The database file is locked\r\ndatabase is locked")
                                        {
                                            Console.WriteLine("Failed :" + exception.Message + "  Now what do we do");
                                            Console.ReadKey();
                                        }
                                        didThisWork = false;
                                        Console.WriteLine("Failed :" + exception.Message);
                                        Console.ReadKey();
                                        #endregion
                                    }
                                    finally
                                    {
                                        //newProfiler.printMemory(counter.ToString() + "O"); counter++;
                                        connection.Close();
                                    }
                                    #endregion

                                    didThisWork = true;//if the Commit worked we will set the toggle to true here
                                }//this using cleans up nicely 3-20-11
                                #endregion
                            }
                            catch (SQLiteException exception)
                            {
                                #region inside catch
                                if (exception.Message == "The database file is locked\r\ndatabase is locked")
                                {
                                    Console.WriteLine("Failed :" + exception.Message + "  Now what do we do" + threadName);
                                    Console.ReadKey();
                                    int counter2 = 0;
                                    while (didThisWork == false)
                                    {
                                        try
                                        {
                                            counter++;
                                            Console.WriteLine("loop " + counter2);
                                            #region inside try
                                            command.ExecuteNonQuery();
                                            using (SQLiteTransaction transaction = connection.BeginTransaction())
                                            {
                                                #region set command string for inserts
                                                //command.CommandText = "INSERT INTO FeatureLiteTable values (@ID, @Abundance, @ChargeState, @DriftTime, @MonoisotopicMass, @RetentionTime, @Score)";
                                                commandTextString = "INSERT INTO TableInfo values (";
                                                for (int i = 0; i < columnList.Count - 1; i++)
                                                {
                                                    commandTextString += "@" + columnList[i] + ",";
                                                }
                                                commandTextString += "@" + columnList[columnList.Count - 1] + ")";

                                                #endregion

                                                command.CommandText = commandTextString;

                                                #region setup columns and add data to each column.  In this case initialize count
                                                for (int i = 0; i < columnList.Count; i++)
                                                {
                                                    DbParameter parameterNumber0 = command.CreateParameter();
                                                    parameterNumber0.ParameterName = columnList[i];
                                                    parameterNumber0.DbType = DbType.Int32;
                                                    parameterNumber0.Value = 0; //sets the count to zero

                                                    command.Parameters.Add(parameterNumber0);
                                                }
                                                command.ExecuteNonQuery();
                                                #endregion

                                                #region commit try catch
                                                try
                                                {
                                                    transaction.Commit();
                                                }
                                                catch (SQLiteException exception2)
                                                {
                                                    #region inside catch
                                                    if (exception.Message == "The database file is locked\r\ndatabase is locked")
                                                    {
                                                        Console.WriteLine("Failed :" + exception2.Message + "  Now what do we do");
                                                        Console.ReadKey();
                                                    }
                                                    didThisWork = false;
                                                    Console.WriteLine("Failed :" + exception.Message);
                                                    Console.ReadKey();
                                                    #endregion
                                                }
                                                finally
                                                {
                                                    connection.Close();
                                                }
                                                #endregion

                                                didThisWork = true;//if the Commit worked we will set the toggle to true here
                                            }//this using cleans up nicely 3-20-11
                                            #endregion
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Failed :" + exception.Message);
                                        }
                                    }
                                }
                                didThisWork = false;
                                Console.WriteLine("Failed :" + exception.Message);
                                Console.ReadKey();
                                #endregion
                            }

                            finally
                            {
                                //newProfiler.printMemory(counter.ToString()); counter++;
                                connection.Close();
                                //newProfiler.printMemory(counter.ToString()+"k"); counter++;
                            }
                            //newProfiler.printMemory(counter.ToString()); counter++;
                            command.Parameters.Clear();
                            //newProfiler.printMemory(counter.ToString()); counter++;
                        }//this using cleans up nicely 3-20-11

                        //connection.Close();//already closed in the finally 
                    }//this using cleans up nicely 3-20-11
                }
                catch (Exception anything)
                {
                    Console.WriteLine("Failed :" + anything.Message + "  Now what do we do");
                }


            }//endlock

            //newProfiler.printMemory("After lock Using " + threadName);
            int text = 4; text = 5; int text3 = text;

            columnList = null;
            return didThisWork;
        }

    }
}

