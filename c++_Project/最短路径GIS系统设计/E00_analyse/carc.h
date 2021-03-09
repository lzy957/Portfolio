#ifndef CARC_H
#define CARC_H
#include "cnode.h"
#include "cproject.h"
#include <QtOpenGL>
#include <list>
using namespace std;
class CArc
{
public:
    CArc();
    int auto_num;
    int user_num;
    CNode *fnode;
    CNode *tnode;
    list<CPoint*> minornodelist;
    int lpoly;
    int rpoly;
    int nodecount;
    void Draw(CProject *proj);
    void Drawpath(CProject *proj);
};

#endif // CARC_H
