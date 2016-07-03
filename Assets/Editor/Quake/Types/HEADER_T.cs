using System;

public struct HEADER_T
{
    public Int32 version;
    public ENTRY_T entities;
    public ENTRY_T planes;
    public ENTRY_T miptex;
    public ENTRY_T vertices;
    public ENTRY_T visilist;
    public ENTRY_T nodes;
    public ENTRY_T texinfos;
    public ENTRY_T faces;
    public ENTRY_T lightmaps;
    public ENTRY_T clipnodes;
    public ENTRY_T leaves;
    public ENTRY_T lface;
    public ENTRY_T edges;
    public ENTRY_T ledges;
    public ENTRY_T models;
}
