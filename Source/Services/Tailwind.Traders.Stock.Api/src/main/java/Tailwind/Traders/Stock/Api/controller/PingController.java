package Tailwind.Traders.Stock.Api.controller;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ResponseStatus;
import springfox.documentation.annotations.ApiIgnore;

@Controller
@ApiIgnore
@ResponseStatus(HttpStatus.OK)
public class PingController {
    @GetMapping("/liveness")
    public ResponseEntity<String> ping() {
        return ResponseEntity
            .status(HttpStatus.OK)
            .body("Healthy");
    }
};