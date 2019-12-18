//
//  SFLogger.hpp
//  RenderingPlugin
//
//  Created by Patrick Metcalfe on 6/19/17.
//  Copyright © 2017 Sturfee. All rights reserved.
//

#ifndef SFLogger_hpp
#define SFLogger_hpp

#include <stdio.h>
#include <string>

#ifdef ANDROID
# include <android/log.h>
#endif/*
android_LogPriority {
ANDROID_LOG_UNKNOWN = 0, ANDROID_LOG_DEFAULT, ANDROID_LOG_VERBOSE, ANDROID_LOG_DEBUG,
ANDROID_LOG_INFO, ANDROID_LOG_WARN, ANDROID_LOG_ERROR, ANDROID_LOG_FATAL,
ANDROID_LOG_SILENT
}
*/

typedef enum : unsigned int {
    info = 0,
    debug,
    warn,
    error,
    fatal,
    unknown
} LogLevel;

class SFLogger {
public:
    static SFLogger& current();
    static constexpr bool shouldIncludeLogs = false;
    static constexpr bool shouldLogLevel(const LogLevel level);
    
    std::string logTag;
    
    SFLogger() : logTag("SturGProcess") {};
    SFLogger(std::string logtag) : logTag(logtag) {};
    ~SFLogger() {};
    
    void log(std::string message);
    void warn(std::string message);
    void debug(std::string message);
    void error(std::string message);
    
    template <LogLevel L>
    void write(std::string message);
};

#endif /* SFLogger_hpp */
