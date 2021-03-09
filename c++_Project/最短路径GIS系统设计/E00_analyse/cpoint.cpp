#include "cpoint.h"

CPoint::CPoint()
{

}

CPoint::CPoint(int xt)
{
    x=xt;
    y=xt;
}

void CPoint::setX(double x)
{
    this->x=x;
}

void CPoint::setY(double y)
{
    this->y=y;
}

void CPoint::setPoint(CPoint pt)
{
    x=pt.x;
    y=pt.y;
}
