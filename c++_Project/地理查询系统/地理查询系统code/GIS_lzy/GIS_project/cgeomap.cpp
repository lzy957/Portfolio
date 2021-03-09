#include "cgeomap.h"

CGeomap::CGeomap()
{

}

CGeomap::~CGeomap()
{
    list<CGeolayer*>::iterator i;
    for(i=this->Geolayers.begin();i!=this->Geolayers.end();++i)
        delete(*i);
}

void CGeomap::Draw()
{
    list<CGeolayer*>::iterator i;
    for( i=this->Geolayers.begin();i!=this->Geolayers.end();++i)
        (*i)->Draw(proj);
}
