#include "crect.h"


CRect::CRect()
{

}

void CRect::Split(CRect* box1,CRect* box2,CRect* box3,CRect* box4)
{
    unsigned int length;
    unsigned int width;
    length=(this->top - this->bottom)/2;
    width=(this->right - this->left)/2;
    box2->bottom=this->bottom+length;
    box2->top=this->top;
    box2->left=this->left;
    box2->right=this->left+width;
    box3->bottom=this->bottom;
    box3->top=this->top-length;
    box3->left=this->left;
    box3->right=this->left+width;
    box4->bottom=this->bottom;
    box4->top=this->top-length;
    box4->left=this->left+width;
    box4->right=this->right;
    box1->bottom=this->bottom+length;
    box1->top=this->top;
    box1->left=this->left+width;
    box1->right=this->right;
}

int CRect::quadrant(CGeopoint pt)
{
    CRect sub[4];
    this->Split(sub,sub+1,sub+2,sub+3);
//    int length=(rect.top-rect.bottom)/2;
//    int width=(rect.right-rect.left)/2;
    if(sub[UR].contain(pt))
        return UR;
    else if(sub[UL].contain(pt))
        return UL;
    else if(sub[LL].contain(pt))
        return LL;
    else if(sub[LR].contain(pt))
        return LR;
    else return -1;
}

int CRect::contain(CGeopoint pt)
{
    int x=pt.x;
    int y=pt.y;
    if(x>this->left&&x<this->right&&y>this->bottom&&y<this->top)
        return 1;
    else
        return 0;

}
