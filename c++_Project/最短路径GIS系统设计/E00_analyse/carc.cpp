#include "carc.h"
#include "gdal_priv.h"

CArc::CArc()
{

}

void CArc::Draw(CProject *proj)
{
    list<CPoint*>::iterator i;
    glBegin(GL_LINE_STRIP);
    glLineWidth(5);
    float r=120;
    float g=130;
    float b=180;
    glColor4f(r/255,g/255,b/255,1);
    proj->towindows(fnode->pt.x,fnode->pt.y);
    glVertex2f(proj->wx,proj->wy);
    for(i=this->minornodelist.begin();i!=this->minornodelist.end();++i)
    {
        proj->towindows((*i)->x,(*i)->y);
        glVertex2f(proj->wx,proj->wy);
        //        float x=(float((*i)->x))/scalex-0.2;
        //        float y=(float((*i)->y))/scaley-0.2;
    }
    proj->towindows(tnode->pt.x,tnode->pt.y);
    glVertex2f(proj->wx,proj->wy);
       glEnd();
    glFlush();
}

void CArc::Drawpath(CProject *proj)
{
    list<CPoint*>::iterator i;
//    glColor3f(1.0, 0.0, 0.0);
//    glLineWidth(100);
    proj->towindows(fnode->pt.x,fnode->pt.y);
//    glBegin(GL_POINTS);
//    glVertex2f(proj->wx,proj->wy);
//    glEnd();

    glBegin(GL_LINE_STRIP);
    glLineWidth(20);
    float r=255;
    float g=0;
    float b=0;
    glColor4f(r/255,g/255,b/255,1);

    glVertex2f(proj->wx,proj->wy);
    for(i=this->minornodelist.begin();i!=this->minornodelist.end();++i)
    {
        proj->towindows((*i)->x,(*i)->y);
        glVertex2f(proj->wx,proj->wy);
        //        float x=(float((*i)->x))/scalex-0.2;
        //        float y=(float((*i)->y))/scaley-0.2;
    }
    proj->towindows(tnode->pt.x,tnode->pt.y);
    glVertex2f(proj->wx,proj->wy);
       glEnd();
    glFlush();
}
