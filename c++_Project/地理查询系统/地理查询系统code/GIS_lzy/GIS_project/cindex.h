#ifndef CINDEX_H
#define CINDEX_H
#include "cnameindex.h"

class CIndex
{
public:
    CIndex();

    list<CNameIndex* > Nameindex;
    void NameIndexset(CCityList fulldataset);
    CChncity* SearchNameIndex(QString input,CCityList fulldataset,CProject* proj);
};

#endif // CINDEX_H
