#ifndef CGEOPOINT_H
#define CGEOPOINT_H
#include "cgeometry.h"
#include <QtOpenGL>

class CGeopoint : public CGeometry
{
public:
    CGeopoint();
    int x;
    int y;
    void DataLoad();
    void Draw(CProject* proj,CSymbol symbol);
};

#endif // CGEOPOINT_H
