import * as $metadata from './metadata.g';
import * as $models from './models.g';
import * as qs from 'qs';
import { AxiosClient, ModelApiClient, ServiceApiClient, ItemResult, ListResult } from 'coalesce-vue/lib/api-client';
import { AxiosPromise, AxiosResponse, AxiosRequestConfig } from 'axios';

export class TriviaBoardApiClient extends ModelApiClient<$models.TriviaBoard> {
  constructor() { super($metadata.TriviaBoard); }
}


export class TriviaServiceApiClient extends ServiceApiClient<typeof $metadata.TriviaService> {
  constructor() { super($metadata.TriviaService); }
  public getRandomTriviaBoard($config?: AxiosRequestConfig): AxiosPromise<ItemResult<$models.TriviaBoard>> {
    const $method = this.$metadata.methods.getRandomTriviaBoard;
    const $params =  {
    };
    return this.$invoke($method, $params, $config);
  }

  public getRandomTriviaBoardWithNoAnswers($config?: AxiosRequestConfig): AxiosPromise<ItemResult<any>> {
    const $method = this.$metadata.methods.getRandomTriviaBoardWithNoAnswers;
    const $params =  {
    };
    return this.$invoke($method, $params, $config);
  }

  public getTriviaBoardOfId(id: number | null, $config?: AxiosRequestConfig): AxiosPromise<ItemResult<$models.TriviaBoard>> {
    const $method = this.$metadata.methods.getTriviaBoardOfId;
    const $params =  {
      id,
    };
    return this.$invoke($method, $params, $config);
  }

}


