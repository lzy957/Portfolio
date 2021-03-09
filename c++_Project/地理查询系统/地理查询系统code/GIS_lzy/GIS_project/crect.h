#ifndef CRECT_H
#define CRECT_H
#include "cgeopoint.h"

/* 一个矩形区域的象限划分：:

       UL(1)   |    UR(0)
     ----------|-----------
       LL(2)   |    LR(3)
以下对该象限类型的枚举
*/
typedef enum
{
    UR = 0,
    UL = 1,
    LL = 2,
    LR = 3
}QuadrantEnum;

class CRect
{
public:
    CRect();
    int right;
    int left;
    int top;
    int bottom;
    void Split(CRect* box1,CRect* box2,CRect* box3,CRect* box4);
    int quadrant(CGeopoint pt);
    int contain(CGeopoint pt);
};

#endif // CRECT_H
