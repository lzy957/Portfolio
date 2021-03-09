#ifndef CNODETABLE_H
#define CNODETABLE_H
#include "carc.h"
using namespace std;

class CNodetable
{
public:
    CNodetable();
    int fnum;
    int arcnum[10];
    vector<CArc*> arclist;
};

#endif // CNODETABLE_H
