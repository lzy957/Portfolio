#include "cchncity.h"
#include "iostream"
#include "strings.h"

using namespace std;

CChncity::CChncity()
{
    this->cv=new Characonvert;
    isutf8=false;
}

CChncity::CChncity(CChncity *a)
{
    if(this!=a)
    {delete cv;
    cv=new Characonvert;
    cv->sFirstLetter=a->cv->sFirstLetter;
    name=a->name;
    }
}

CChncity::~CChncity()
{
    x=0;y=0;
    if(this->cv!=0)
    {
        delete(cv);
        cv=0x00;
    }
    this->name="";
}
void CChncity::Display()
{
    cout<<"City name："<<this->name<<endl;
    cout<<"coordination x:"<<this->x<<endl;
    cout<<"coordination y:"<<this->y<<endl;
}

void CChncity::Draw(CProject* proj)
{
//    glMatrixMode(GL_PROJECTION);
//    glLoadIdentity();
//    glOrtho(-200, 200, -200, 200, -1, 1);

    glPointSize(5);
    glBegin(GL_POINTS);
    glColor3f(0.0,0.0,0.0);
//    glVertex2f(this->x,this->y);
//    glVertex2f((float(this->x))/scalex-0.2,(float(this->y))/scaley-0.2);
    proj->towindows(this->x,this->y);
    glVertex2f(proj->wx,proj->wy);
    glEnd();
    glEnable(GL_POINT_SMOOTH);
    glEnable(GL_BLEND);
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    glFlush();
}

void CChncity::Mark(CProject* proj)
{
    glPointSize(100);
    glBegin(GL_POINTS);
    glColor3f(0.5,0.0,0.0);
//    glVertex2f((float(this->x))/scalex-0.2,(float(this->y))/scaley-0.2);
    proj->towindows(this->x,this->y);
    glVertex2f(proj->wx,proj->wy);
    glEnd();
    glEnable(GL_POINT_SMOOTH);
    glEnable(GL_BLEND);
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    glFlush();
}
