#if defined(__ANDROID__)

#include <exception>

namespace boost
{
    void throw_exception(std::exception const&) {}
}

#endif
