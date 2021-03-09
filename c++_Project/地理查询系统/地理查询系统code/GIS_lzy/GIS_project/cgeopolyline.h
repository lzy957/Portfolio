#ifndef CGEOPOLYLINE_H
#define CGEOPOLYLINE_H
#include "cgeometry.h"
#include "cgeopoint.h"
#include <list>
using namespace std;

class CGeopolyline: public CGeometry
{
public:
    CGeopolyline();
    ~CGeopolyline();
    list<CGeopoint*> Polyline;
    void DataLoad();
    void Draw(CProject *proj,CSymbol symbol);
};

#endif // CGEOPOLYLINE_H
