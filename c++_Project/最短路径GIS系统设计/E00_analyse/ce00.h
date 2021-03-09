#ifndef CE00_H
#define CE00_H
#include "carc.h"
#include "cexp.h"
#include "ctol.h"
#include "clog.h"
#include "cnodetable.h"
#include "cattribution.h"

class CE00
{
public:
    CE00();
    list<CArc*> arclist;
    CEXP *EXP;
    int arcprecision;
    CTOL *TOL;
    int sin;
    vector<CLOG*> loglist;
    CAttribution *cattri;
    vector<CNodetable*> nodetable;
    void Draw(CProject *prj);
    double **datamatrix;
    double dmatrix[892][892]={0};
    int pointnum;
    vector<CPoint*> pointarray;
    void initialnode(int num);
};

#endif // CE00_H
