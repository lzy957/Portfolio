#include "cgeopolygon.h"

CGeopolygon::CGeopolygon()
{

}

CGeopolygon::~CGeopolygon()
{
    list<CGeopoint*>::iterator i;
    for( i=this->Polygon.begin();i!=this->Polygon.end();++i)
        delete[]*i;
}

void CGeopolygon::Draw(CProject *proj, CSymbol symbol)
{
    if(symbol.isfilled==1)
    {
        float r=(float)symbol.fR;
        float g=symbol.fG;
        float b=symbol.fB;
        glColor4f(r/255,g/255,b/255,1);
        glBegin(GL_POLYGON);
        list<CGeopoint*>::iterator i;
        for(i=this->Polygon.begin();i!=this->Polygon.end();++i)
        {
            proj->towindows((*i)->x,(*i)->y);
            glVertex2f(proj->wx,proj->wy);
//            float x=(float((*i)->x))/scalex-0.2;
//            float y=(float((*i)->y))/scaley-0.2;
//            glVertex2f(x,y);
        }
        glEnd();
        glFlush();
    }
//    glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);
}
