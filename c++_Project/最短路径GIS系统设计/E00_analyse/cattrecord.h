#ifndef CATTRECORD_H
#define CATTRECORD_H
#include <vector>
using namespace std;

class CAttrecord
{
public:
    CAttrecord();
    int fnodenum;
    int tnodenum;
    int lpolynum;
    int rpolynum;
    float length;
    int autorivernum;
    int riverid;
    int type;
    vector<double> attrirecord;
};

#endif // CATTRECORD_H
