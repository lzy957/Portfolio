#ifndef CPOINT_H
#define CPOINT_H


class CPoint
{
public:
    CPoint();
    CPoint(int xt);
    double x;
    double y;
    void setX(double x);
    void setY(double y);
    void setPoint(CPoint pt);
};

#endif // CPOINT_H
