#include <cstdio>
#include <iostream>
#include <memory>
#include <stdexcept>
#include <string>
#include <array>
#include <stdio.h>

std::string exec(const char* cmd) {
    std::array<char, 128> buffer;
    std::string result;
    std::unique_ptr<FILE, decltype(&_pclose)> pipe(_popen(cmd, "r"), _pclose);
    if (!pipe) {
        throw std::runtime_error("popen() не работает!");
    }
    while (fgets(buffer.data(), buffer.size(), pipe.get()) != nullptr) {
        result += buffer.data();
    }
    return result;
}

int main() {
    setlocale(LC_ALL, "ru");
    std::string domain = "google.com";
    std::string command = "nslookup " + domain;
    try {
        std::string output = exec(command.c_str());
        std::cout << "Вывод nslookup:\n" << output << std::endl;
    }
    catch (const std::exception& e) {
        std::cerr << "Ошибка запуска команды: " << e.what() << std::endl;
    }
    return 0;
}
