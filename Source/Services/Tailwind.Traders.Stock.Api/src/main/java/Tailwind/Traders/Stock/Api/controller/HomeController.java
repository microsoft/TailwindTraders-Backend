package Tailwind.Traders.Stock.Api.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import springfox.documentation.annotations.ApiIgnore;

@Controller
@ApiIgnore
public class HomeController {
    @GetMapping("/")
    public String redirect() {
        return "redirect:/swagger-ui.html";
    }
}

