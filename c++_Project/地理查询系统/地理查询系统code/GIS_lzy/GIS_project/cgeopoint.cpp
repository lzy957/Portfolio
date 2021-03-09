#include "cgeopoint.h"

CGeopoint::CGeopoint()
{

}

void CGeopoint::Draw(CProject *proj, CSymbol symbol)
{
    glBegin(GL_POINTS);
    glColor3f(0.0,0.0,0.0);
    proj->towindows(x,y);
    glVertex2f(proj->wx,proj->wy);
//    glVertex2f((float(this->x))/proj,(float(this->y))/scaley);
    glEnd();
    glFlush();
}
