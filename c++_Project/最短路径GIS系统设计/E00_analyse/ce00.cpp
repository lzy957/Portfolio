#include "ce00.h"

CE00::CE00()
{

}

void CE00::Draw(CProject *prj)
{
    list<CArc*>::iterator i;
    for( i=this->arclist.begin();i!=this->arclist.end();++i)
        (*i)->Draw(prj);
}

void CE00::initialnode(int num)
{
    for(int i=0;i<num;i++)
    {
        CPoint *pt=new CPoint(0);
        pointarray.push_back(pt);
    }
}
