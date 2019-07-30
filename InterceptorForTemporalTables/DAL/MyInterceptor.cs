using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace InterceptorForTemporalTables.DAL
{
    public class MyInterceptor : IDbCommandInterceptor
    {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //command.CommandText = command.CommandText.Replace("SELECT","SeLeCt top(1)");

            if (interceptionContext.ObjectContexts.Any())
            {
                foreach(var contextObject in interceptionContext.ObjectContexts)
                {
                    if (contextObject.DefaultContainerName=="SchoolContext")
                    {
                        foreach (var context in contextObject.InterceptionContext.DbContexts)
                        {
                            if (context is SchoolContext)
                            {
                                var myObjRef = (SchoolContext)context;
                                if (myObjRef.PointInTime != null && command.CommandText.IndexOf("SYSTEM_TIME") == -1
                                    && command.CommandText.IndexOf("UPDATE") == -1 && command.CommandText.IndexOf("INSERT") == -1
                                    && command.CommandText.IndexOf("DELETE") == -1 && command.CommandText.IndexOf("version") == -1)
                                {
                                    string pattern = @"(FROM[ \t]+\[[a-zA-Z]{3}\].\[\w+\])";
                                    var rgx = new Regex(pattern);
                                    var matches = rgx.Matches(command.CommandText);
                                    int index = -1;
                                    int length = 0;

                                    for(int i = matches.Count; i > 0; i--)
                                    {
                                        index = matches[i-1].Index;
                                        length = matches[i-1].Length;
                                        command.CommandText = command.CommandText.Insert(index + length, " FOR SYSTEM_TIME AS OF  '" + myObjRef.PointInTime + "' ");
                                    }

                                    pattern = @"(JOIN[ \t]+\[[a-zA-Z]{3}\].\[\w+\])";
                                    rgx = new Regex(pattern);
                                    matches = rgx.Matches(command.CommandText);
                                    index = -1;
                                    length = 0;

                                    for (int i = matches.Count; i > 0; i--)
                                    {
                                        index = matches[i - 1].Index;
                                        length = matches[i - 1].Length;
                                        command.CommandText = command.CommandText.Insert(index + length, " FOR SYSTEM_TIME AS OF  '" + myObjRef.PointInTime + "' ");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            //command.CommandText = command.CommandText.Replace("SELECT", "Select top(1)");
        }
    }
}