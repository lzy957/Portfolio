#ifndef CLOG_H
#define CLOG_H
#include <string>
using namespace std;
class CLOG
{
public:
    CLOG();
    int year;
    int month;
    int day;
    int hours;
    int min;
    int connecttimem;
    int cputimes;
    int iotimes;
    string commandline;
};

#endif // CLOG_H
