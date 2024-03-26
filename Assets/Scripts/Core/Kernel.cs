using CommandTerminal;
using System;
using System.Collections.Generic;

public class Kernel
{
    private static Kernel _instance;
    public static Kernel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Kernel();
            }
            return _instance;
        }
    }

    private Kernel() { }
}
