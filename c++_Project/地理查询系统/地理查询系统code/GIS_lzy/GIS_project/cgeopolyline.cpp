#include "cgeopolyline.h"

CGeopolyline::CGeopolyline()
{

}

CGeopolyline::~CGeopolyline()
{
    list<CGeopoint*>::iterator i;
    for( i=this->Polyline.begin();i!=this->Polyline.end();++i)
        delete(*i);
}

void CGeopolyline::Draw(CProject *proj, CSymbol symbol)
{
    if(symbol.isline==1)
    {
    list<CGeopoint*>::iterator i;
    glBegin(GL_LINE_STRIP);
    glLineWidth(symbol.linewidth);
    float r=symbol.lR;
    float g=symbol.lG;
    float b=symbol.lB;
    glColor4f(r/255,g/255,b/255,1);
    for(i=this->Polyline.begin();i!=this->Polyline.end();++i)
    {
        proj->towindows((*i)->x,(*i)->y);
//        float x=(float((*i)->x))/scalex-0.2;
//        float y=(float((*i)->y))/scaley-0.2;
        glVertex2f(proj->wx,proj->wy);
    }
       glEnd();
    glFlush();
}
//    glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);
}
