using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternCreator
{
    private string fileTemplate = "#pragma kernel CSMain\n" +
            "\n" +
            "RWTexture2D<float4> Result;\n" +
            "\n" +
            "float xPos;\n" +
            "float yPos;\n" +
            "\n" +
            "float4 color;\n" +
            "\n" +
            "bool lr;\n" +
            "bool ud;\n" +
            "\n" +
            "[numthreads(1, 1, 1)]\n" +
            "void CSMain(uint3 id : SV_DispatchThreadID)\n" +
            "{\n" +
            "    float xMult = 1;\n" +
            "    float yMult = 1;\n" +
            "\t\n" +
            "    if (lr)\n" +
            "    {\n" +
            "        xMult = -1;\n" +
            "    }\n" +
            "\t\n" +
            "    if (ud)\n" +
            "    {\n" +
            "        yMult = -1;\n" +
            "    }\n\n";

    private string resultTemplate = "\tResult[int2(xPos$, yPos#)] = color;";

    private string fileName;
    void CreateFile(string fileName)
    {
        this.fileName = fileName;
    }
}
