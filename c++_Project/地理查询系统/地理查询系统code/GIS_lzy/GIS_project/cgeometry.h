#ifndef CGEOMETRY_H
#define CGEOMETRY_H
#include "csymbol.h"
#include "cproject.h"

class CGeometry
{
public:
    CGeometry();
    int type;
//    virtual void DataLoad();
    virtual void Draw(CProject* proj,CSymbol symbol);
};

#endif // CGEOMETRY_H
