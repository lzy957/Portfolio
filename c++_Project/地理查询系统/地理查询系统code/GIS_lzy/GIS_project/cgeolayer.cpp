#include "cgeolayer.h"
#include "cgeopolyline.h"
#include "cgeopolygon.h"

CGeolayer::CGeolayer()
{
    this->layername=new char(20);
    symbol=new CSymbol;
}

CGeolayer::~CGeolayer()
{
    list<CGeometry*>::iterator i;
    for(i=this->Geoobjects.begin();i!=this->Geoobjects.end();++i)
        delete(*i);
}

void CGeolayer::Draw(CProject* proj)
{
    list<CGeometry*>::iterator i;
//    if(this->symbol->isline)
//    {
        for(i=this->Geoobjects.begin();i!=this->Geoobjects.end();++i)
           (*i)->Draw(proj,*this->symbol);
//    }
//    if(this->symbol->isfilled)
//    {
//        for(i=this->Geoobjects.begin();i!=this->Geoobjects.end();++i)
//            (*i)->Draw(scalex,scaley,*this->symbol);
//    }
}
