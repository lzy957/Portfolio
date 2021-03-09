#ifndef CATTRIBUTION_H
#define CATTRIBUTION_H
#include <string>
#include <list>
#include "cattrecord.h"
using namespace std;

typedef struct attri
{
    string name;
    int sizebina;
    int startpos;
    int ofwidth;
    int ofprecision;
    int type;
    string sfname;
    int attrindex;
}attristruct;

class CAttribution
{
public:
    CAttribution();
    int infoprecision;
    string name;
    string externalflag;
    int attrinum;
    int attrinumtotal;
    int datalength;
    int recordnum;
    list<attristruct *> attrilist;
    list<CAttrecord *> attrecordlist;
};

#endif // CATTRIBUTION_H
