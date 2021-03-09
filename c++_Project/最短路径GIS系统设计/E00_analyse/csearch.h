#ifndef CSEARCH_H
#define CSEARCH_H
#include "ce00.h"
#include "list"
using namespace std;

class CSearch
{
public:
    CSearch();
    ~CSearch();
    bool findtonode(int numf, int numt, CE00 * ce00);
    bool shortpath(int k, CE00 *ce00);
    int acc;
    double length_temp[100]={0};
    int bcc;
    int path_tempt[100]={0};
    int path[100]={0};
    double length;
    int endnum;
    int fromnum;
    int countg;
    int count;
    int tempt[100]={0};
    void clear();
    list<CArc*> pathlist;
    void SetList(CE00 *e00);
    void Draw(CProject *prj);
};

#endif // CSEARCH_H
