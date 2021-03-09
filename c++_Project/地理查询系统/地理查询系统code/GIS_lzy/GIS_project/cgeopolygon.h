#ifndef CGEOPOLYGON_H
#define CGEOPOLYGON_H
#include "cgeometry.h"
#include "cgeopoint.h"
#include <list>
//#include <QString>
using namespace std;

class CGeopolygon : public CGeometry
{
public:
    CGeopolygon();
    ~CGeopolygon();
    list<CGeopoint*> Polygon;
    void DataLoad();
    void Draw(CProject* proj,CSymbol symbol);
};

#endif // CGEOPOLYGON_H
