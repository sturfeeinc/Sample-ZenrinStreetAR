//
//  SFDataStream.hpp
//  SturgProcess
//
//  Created by Patrick Metcalfe on 3/20/18.
//  Copyright © 2018 Sturfee Inc. All rights reserved.
//

#ifndef SFDataStream_hpp
#define SFDataStream_hpp

#include <stdio.h>
#include <algorithm>
#include <ostream>
#include <streambuf>

template < typename Char, typename Traits = std::char_traits<Char> >
class SFDataStreamBuffer
:   public std::basic_streambuf<Char, Traits>
{
    // Types
    // =====
    
private:
    typedef std::basic_streambuf<Char, Traits> Base;
    
public:
    typedef typename Base::char_type char_type;
    typedef typename Base::int_type int_type;
    typedef typename Base::pos_type pos_type;
    typedef typename Base::off_type off_type;
    typedef typename Base::traits_type traits_type;
    typedef const char_type* pointer;
    typedef std::size_t size_type;
    
    // Construction
    // ============
    
public:
    SFDataStreamBuffer(pointer data, size_type size) {
        // These casts are safe (no modification will take place):
        char* begin = const_cast<char_type*>(data);
        char* end = const_cast<char_type*>(data + size);
        this->setg(begin, begin, end);
    }
    
    // Stream Buffer Interface
    // =======================
    
protected:
    virtual std::streamsize showmanyc();
    virtual std::streamsize xsgetn(char_type*, std::streamsize);
    virtual int_type pbackfail(int_type);
    
    // Utilities
    // =========
    
protected:
    int_type eof() { return traits_type::eof(); }
    bool is_eof(int_type ch) { return ch == eof(); }
};

template <typename Char, typename Traits>
std::streamsize
SFDataStreamBuffer<Char, Traits>::showmanyc() {
    return this->egptr() - this->gptr();
}

template <typename Char, typename Traits>
std::streamsize
SFDataStreamBuffer<Char, Traits>::xsgetn(char_type* p, std::streamsize n) {
	std::streamsize result = std::min<std::streamsize>(n, (this->egptr() - this->gptr()));
    std::copy(this->gptr(), this->gptr() + result, p);
    this->gbump((int)result);
    return result;
}

template <typename Char, typename Traits>
typename SFDataStreamBuffer<Char, Traits>::int_type
SFDataStreamBuffer<Char, Traits>::pbackfail(int_type ch) {
    if(is_eof(ch)) {
        if(this->eback() != this->gptr()) {
            this->gbump(-1);
            return traits_type::to_int_type(*this->gptr());
        }
    }
    return eof();
}

typedef SFDataStreamBuffer<char> SFDataSequenceInputBuffer;

template < typename Char, typename Traits = std::char_traits<Char> >
class SFDataStream
:   public std::basic_istream<Char, Traits>
{
private:
    typedef std::basic_istream<Char, Traits> Base;
    
public:
    typedef typename Base::char_type char_type;
    typedef typename Base::int_type int_type;
    typedef typename Base::pos_type pos_type;
    typedef typename Base::off_type off_type;
    typedef typename Base::traits_type traits_type;
    
private:
    typedef SFDataStreamBuffer<Char, Traits> buffer_type;
    
public:
    typedef typename buffer_type::pointer pointer;
    typedef typename buffer_type::size_type size_type;
    
    // Construction
    // ============
    
public:
    explicit SFDataStream(pointer data, size_type size)
    :   Base(&m_buf), m_buf(data, size)
    {}
    
private:
    buffer_type m_buf;
};

typedef SFDataStream<char> SFDataSequenceInputStream;


#endif /* SFDataStream_hpp */
