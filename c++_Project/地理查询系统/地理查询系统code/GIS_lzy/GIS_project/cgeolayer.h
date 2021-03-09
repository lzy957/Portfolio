#ifndef CGEOLAYER_H
#define CGEOLAYER_H
#include "cgeometry.h"
#include <list>
#include <string>
#include "clistsymbol.h"
#include "cproject.h"
using namespace std;

class CGeolayer
{
public:
    CGeolayer();
    ~CGeolayer();
    list<CGeometry*> Geoobjects;
    int namesize;
    string layername;
    int objectnum;
    CSymbol* symbol;
    void Draw(CProject* proj);
};

#endif // CGEOLAYER_H
