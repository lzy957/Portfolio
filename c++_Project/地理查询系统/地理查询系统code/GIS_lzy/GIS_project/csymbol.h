#ifndef CSYMBOL_H
#define CSYMBOL_H
#include <string>
using namespace std;

class CSymbol
{
public:
    CSymbol();
    string layernames;
    int objectnum;
    bool isline;
    double linewidth;
    int lR,lG,lB;
    bool isfilled;
    int fR,fG,fB;
};

#endif // CSYMBOL_H
