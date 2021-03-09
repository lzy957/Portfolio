#include "cproject.h"

CProject::CProject()
{

}

void CProject::towindows(int x, int y)
{
    float xf=(float)x;
    float yf=(float)y;
    wx=(xf/scalex-0.2)*1.5;
    wy=(yf/scaley-0.2)*1.5;
}

void CProject::toworld(float x, float y)
{
    ox=(x/1.5+0.2)*scalex;
    oy=(y/1.5+0.2)*scaley;
}

void CProject::toUI(int x,int y)
{
    float xf=(float)x;
    float yf=(float)y;
    wx=(xf/scalex-0.2)*1.5+1;
    wy=(yf/scaley-0.2)*1.5;
    uix=wx*410;
    uiy=-wy*310+310;
}

void CProject::uitoworld(float x, float y)
{
    ox=((x-410)/410/1.5+0.2)*scalex;
    oy=((-(y-310)/310)/1.5+0.2)*scaley;

}
