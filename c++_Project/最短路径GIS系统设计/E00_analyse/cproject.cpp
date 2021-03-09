#include "cproject.h"

CProject::CProject()
{

}

void CProject::towindows(double x,double y)
{
    wx=((x-xmin)/scalex-0.5)*1.5;
    wy=((y-ymin)/scaley-0.5)*1.5;
}

void CProject::toworld(double x, double y)
{
    ox=(x/1.5+0.2)*scalex;
    oy=(y/1.5+0.2)*scaley;
}

void CProject::toUI(int x,int y)
{
//    float xf=(float)x;
//    float yf=(float)y;
    wx=((x-xmin)/scalex-0.5)*1.5;
    wy=((y-ymin)/scaley-0.5)*1.5;
    uix=(wx+1)*410;
    uiy=(-wy+1)*310;
}

void CProject::uitoworld(float x, float y)
{
    ox=((x-410)/410/1.5+0.2)*scalex;
    oy=((-(y-310)/310)/1.5+0.2)*scaley;

}
